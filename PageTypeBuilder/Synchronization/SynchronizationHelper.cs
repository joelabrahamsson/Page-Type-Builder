namespace PageTypeBuilder.Synchronization
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Web.Hosting;
    using Configuration;
    using EPiServer.Data.Cache;
    using Reflection;

    internal static class SynchronizationHelper
    {
        private static readonly Guid SyncGuid = new Guid("14f94889-d8e3-4294-b84f-9c5d990b019d");
        private static readonly Guid SyncCacheGuid = new Guid("9F85BF43-2509-48B6-AE3D-B75DE9918542");
        private const string SynchornizingStatusText = "Synchornizing";
        private const string PageTypeBuilderSynchornization = "PageTypeBuilderSynchornization";
        private const string ConnectionStringName = "EPiServerDB";
        private static string _siteId;
        private static DirectoryInfo _logFileDirectory;
        private static bool _logFileDirectoryRetrieved;

        public static bool OneTimeSynchornizationEnabled
        {
            get
            {
                return PageTypeBuilderConfiguration.GetConfiguration().OneTimeSynchornizationEnabled &&
                       ConfigurationManager.ConnectionStrings[ConnectionStringName].ProviderName.Equals("System.Data.SqlClient");
            }
        }

        public static IAssemblyLocator AssemblyLocator { get; set; }

        public static bool IsBeingSynchronized(out bool iAmSynching)
        {
            LogInfo("Checking whether synchornization is in progress");
            iAmSynching = false;
            bool isBeingSynchronized = false;

            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings[ConnectionStringName].ConnectionString))
                {
                    connection.Open();
                    bool commit = false;

                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        string sql = string.Format("SELECT COUNT(*) FROM tblItem WITH (TABLOCKX) WHERE pkID = '{0}'", SyncGuid);

                        using (SqlCommand command = new SqlCommand(sql, connection, transaction))
                        {
                            int count = (int)command.ExecuteScalar();

                            if (count == 0)
                            {
                                LogInfo("Creating new record in tblItem as we are now synching");
                                SaveSynchornizationStatus(connection, transaction, SyncGuid, Encoding.UTF8.GetBytes(SynchornizingStatusText));
                                iAmSynching = true;
                                isBeingSynchronized = true;
                                commit = true;
                            }
                            else
                            {
                                string value = GetSynchornizationStatus(connection, transaction, SyncGuid);

                                if (value.Trim() == SynchornizingStatusText)
                                {
                                    LogInfo("Already being synched by another site");
                                    isBeingSynchronized = true;
                                }
                                else
                                {
                                    string timestamps = GetAssemblyTimeStamps();

                                    LogInfo("current application assembly timestamps: " + timestamps + "|");
                                    LogInfo("current timestamps in database:" + value.Trim() + "|");

                                    if (!string.Equals(value.Trim(), timestamps))
                                    {
                                        LogInfo("time stamps have changed, so now synching");
                                        SaveSynchornizationStatus(connection, transaction, SyncGuid, Encoding.UTF8.GetBytes(SynchornizingStatusText));
                                        iAmSynching = true;
                                        isBeingSynchronized = true;
                                        commit = true;
                                    }
                                }
                            }
                        }

                        if (commit)
                            transaction.Commit();
                        else
                            transaction.Rollback();
                    }

                    connection.Close();
                }
            }
            catch (Exception)
            {
                RevertSyncronizationStatus();
                throw;
            }
            return isBeingSynchronized;
        }

        public static void SynchingComplete()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings[ConnectionStringName].ConnectionString))
                {
                    connection.Open();

                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        string sql = string.Format("SELECT COUNT(*) FROM tblItem WITH (TABLOCKX) WHERE pkID = '{0}'", SyncGuid);

                        using (SqlCommand command = new SqlCommand(sql, connection, transaction))
                        {
                            command.ExecuteScalar();
                            SaveSynchornizationStatus(connection, transaction, SyncGuid, Encoding.UTF8.GetBytes(GetAssemblyTimeStamps()));
                            LogInfo("Updated timestamps with value: " + GetAssemblyTimeStamps());
                        }

                        transaction.Commit();
                    }

                    connection.Close();
                }
            }
            catch (Exception)
            {
                RevertSyncronizationStatus();
                throw;
            }
        }

        private static void SaveSynchornizationStatus(SqlConnection connection, SqlTransaction transaction, Guid id, byte[] data)
        {
            using (SqlCommand command = new SqlCommand("[dbo].[ItemSave]", connection, transaction))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", id.ToString());
                command.Parameters.AddWithValue("@DbSchemaId", 1);
                command.Parameters.AddWithValue("@Name", PageTypeBuilderSynchornization);
                command.Parameters.Add("@ItemData", SqlDbType.Image);
                command.Parameters["@ItemData"].Value = data;
                command.ExecuteNonQuery();
            }
        }

        private static string GetSynchornizationStatus(SqlConnection connection, SqlTransaction transaction, Guid id)
        {
            using (SqlCommand command = new SqlCommand("[dbo].[ItemLoad]", connection, transaction))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", id.ToString());
                byte[] bytes = (byte[])command.ExecuteScalar();
                string value = Encoding.UTF8.GetString(bytes);
                LogInfo("tblItem value is currently: " + value);
                return value;
            }
        }

        public static void RevertSyncronizationStatus()
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings[ConnectionStringName].ConnectionString))
            {
                connection.Open();

                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    string sql = string.Format("SELECT COUNT(*) FROM tblItem WITH (TABLOCKX) WHERE pkID = '{0}'", SyncGuid);

                    using (SqlCommand command = new SqlCommand(sql, connection, transaction))
                    {
                        command.ExecuteScalar();
                        SaveSynchornizationStatus(connection, transaction, SyncGuid, new byte[0]);
                        LogInfo("Reverted synchronization status.");
                    }

                    transaction.Commit();
                }

                connection.Close();
            }
        }

        public static void UpateSynchronizationCache(List<int> pageTypeIds, List<int> pageDefinitionIds, List<int> updatedTabDefinitionIds,
            List<string> updatedPropertySettingsCacheKeys, List<Guid> globalPropertySettingsIds)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                AppendItems(stringBuilder, "pageTypeIds", pageTypeIds.Select(c => c as object).ToArray());
                AppendItems(stringBuilder, "pageDefinitionIds", pageDefinitionIds.Select(c => c as object).ToArray());
                AppendItems(stringBuilder, "tabDefinitionIds", updatedTabDefinitionIds.Select(c => c as object).ToArray());
                AppendItems(stringBuilder, "updatedPropertySettingsCacheKeys", updatedPropertySettingsCacheKeys.Select(c => c as object).ToArray());
                AppendItems(stringBuilder, "updatedGlobalPropertySettingsIds", globalPropertySettingsIds.Select(c => c as object).ToArray());

                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings[ConnectionStringName].ConnectionString))
                {
                    connection.Open();
                    SaveSynchornizationStatus(connection, null, SyncCacheGuid, Encoding.UTF8.GetBytes(stringBuilder.ToString()));
                    connection.Close();
                }
            }
            catch
            {
            }
        }

        private static void AppendItems(StringBuilder stringBuilder, string key, IEnumerable<object> items)
        {
            stringBuilder.AppendFormat("{0}:", key);

            foreach (object item in items)
                stringBuilder.AppendFormat("{0}|", item);

            stringBuilder.AppendLine();
        }

        public static void InvalidateCache()
        {
            string value = string.Empty;

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings[ConnectionStringName].ConnectionString))
            {
                connection.Open();
                value = GetSynchornizationStatus(connection, null, SyncCacheGuid);
                connection.Close();
            }

            if (string.IsNullOrEmpty(value))
                return;

            string[] parts = value.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            string part;

            // page types
            if (!string.IsNullOrEmpty(part = GetCacheInvalidationKeyPart(parts, "pageTypeIds", 0)))
            {
                IEnumerable<int> pageTypeIds = part.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);

                if (pageTypeIds.Any())
                    EPiServer.DataAbstraction.PageType.ClearCache();
            }

            // page definitions
            if (!string.IsNullOrEmpty(part = GetCacheInvalidationKeyPart(parts, "pageDefinitionIds", 1)))
            {
                List<int> pageDefinitionIds = part.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();

                if (pageDefinitionIds.Any())
                {
                    foreach (int pageDefinitionId in pageDefinitionIds)
                    {
                        try
                        {
                            EPiServer.DataAbstraction.PageDefinition pageDefinition = EPiServer.DataAbstraction.PageDefinition.Load(pageDefinitionId);
                            pageDefinition.ClearCache();
                        }
                        catch
                        {
                        }
                    }
                }
            }

            // tab definitions
            if (!string.IsNullOrEmpty(part = GetCacheInvalidationKeyPart(parts, "tabDefinitionIds", 2)))
            {
                IEnumerable<int> tabDefinitionIds = part.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);

                if (tabDefinitionIds.Any())
                    EPiServer.DataAbstraction.TabDefinition.ClearCache();
            }

            // property settings and property settings referencing global
            if (!string.IsNullOrEmpty(part = GetCacheInvalidationKeyPart(parts, "updatedPropertySettingsCacheKeys", 3)))
            {
                List<string> cacheKeys = part.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries).ToList();

                foreach (string cacheKey in cacheKeys)
                    EPiServer.CacheManager.Remove(cacheKey);
            }

            // global property settings
            if (!string.IsNullOrEmpty(part = GetCacheInvalidationKeyPart(parts, "updatedGlobalPropertySettingsIds", 4)))
            {
                IEnumerable<Guid> ids = part.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Select(c => new Guid(c));

                foreach (Guid id in ids)
                    CacheProvider.Instance.Remove(id.ToString());
            }
        }

        private static string GetCacheInvalidationKeyPart(IList<string> parts, string key, int index)
        {
            if (parts.Count <= index || !parts[index].StartsWith(key + ":"))
                return string.Empty;

            return parts[index].Substring(parts[index].IndexOf(":") + 1);
        }

        private static string GetAssemblyTimeStamps()
        {
            StringBuilder timeStamps = new StringBuilder();
            IEnumerable<Assembly> assemblies = AssemblyLocator.AssembliesWithReferenceToAssemblyOf<TypedPageData>();

            if (PageTypeBuilderConfiguration.GetConfiguration().OneVersionSynchronizationAssemblyVersionStamp == PageTypeBuilderConfiguration.AssemblyVersionStamp.Version)
            {
                foreach (Assembly assembly in assemblies)
                    timeStamps.AppendFormat("{0} - {1}|", assembly.GetName().Name, assembly.GetName().Version);
            }
            else
            {
                DirectoryInfo binDirectory = new DirectoryInfo(HostingEnvironment.MapPath("~/bin"));

                foreach (FileInfo assemblyFile in binDirectory.GetFiles("*.dll")
                   .Where(current => assemblies.Any(assembly => string.Equals(assembly.GetName().Name, current.Name.Substring(0, current.Name.Length - 4), StringComparison.OrdinalIgnoreCase)))
                   .OrderBy(current => current.Name))
                {
                    timeStamps.AppendFormat("{0} - {1}|", assemblyFile.Name, assemblyFile.LastWriteTime.ToString("yyyyMMdd hh:mm:ss"));
                }   
            }

            return timeStamps.ToString();
        }

        private static void LogInfo(string message)
        {
            DirectoryInfo logDirectory = GetLogDirectory();

            if (logDirectory == null || !logDirectory.Exists)
                return;

            if (_siteId == null)
                _siteId = GetSiteId();

            try
            {
                message = string.Format("{0} - {1}: {2}{2}", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"), message, Environment.NewLine);
                string logFileName = GetLogFileName(logDirectory, _siteId);
                File.AppendAllText(logFileName, message);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error while trying to log messages using PageTypeBuilder.SynchronizationHelper. " + ex.Message);
            }
        }

        public static void ClearLogInfo()
        {
            DirectoryInfo logDirectory = GetLogDirectory();

            if (logDirectory == null || !logDirectory.Exists)
                return;

            string siteId = GetSiteId();
            string logFileName = GetLogFileName(logDirectory, siteId);

            if (!File.Exists(logFileName))
                return;

            try
            {
                File.Delete(logFileName);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("Error while trying to delete file '{0}'. {1}", logFileName, ex.Message));
            }
        }

        private static DirectoryInfo GetLogDirectory()
        {
            if (_logFileDirectoryRetrieved && (_logFileDirectory == null || !_logFileDirectory.Exists))
                return null;

            _logFileDirectoryRetrieved = true;
            string directoryPath = PageTypeBuilderConfiguration.GetConfiguration().OneTimeSynchronizationLogFileDirectoryPath;

            if (string.IsNullOrEmpty(directoryPath))
                return null;

            if (!directoryPath.EndsWith(@"\"))
                directoryPath += @"\";

            _logFileDirectory = new DirectoryInfo(directoryPath);
            return !_logFileDirectory.Exists ? null : _logFileDirectory;
        }

        private static string GetSiteId()
        {
            try
            {
                return string.Format("{0}_{1}", HostingEnvironment.ApplicationHost.GetSiteName(), HostingEnvironment.ApplicationHost.GetSiteID());
            }
            catch
            {
                return string.Empty;
            }
        }

        private static string GetLogFileName(DirectoryInfo logDirectory, string siteId)
        {
            return string.Format("{0}PageTypeBuilderOneTimeSynchornizationLog_{1}.txt", logDirectory.FullName, siteId);
        }

    }
}
