using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using PageTypeBuilder.Configuration;
using Xunit;

namespace PageTypeBuilder.Tests.Configuration
{
    public class PageTypeBuilderConfigurationTests
    {
        private readonly string assemblyConfigFile =AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
        private readonly string disableSyncFile =
            Path.Combine(Path.GetDirectoryName(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile), "DisablePageTypeSync.config");
        
        [Fact]
        public void DisablePageTypeUpdation_DefaultsToFalse()
        {
            var configuration = new PageTypeBuilderConfiguration();
            
            Assert.Equal(false, configuration.DisablePageTypeUpdation);
        }

        [Fact]
        public void GivenPageTypeUpdationDisabledInAppConfig_DisablePageTypeUpdation_ReturnsTrue()
        {
            RecreateConfigSectionWithSingleAttribute("disablePageTypeUpdation", "true");

            PageTypeBuilderConfiguration configuration = PageTypeBuilderConfiguration.GetConfiguration();

            Assert.Equal(true, configuration.DisablePageTypeUpdation);
        }

        [Fact]
        public void GivenPageTypeUpdationFalseInAppConfig_DisablePageTypeUpdation_ReturnsFalse()
        {
            RecreateConfigSectionWithSingleAttribute("disablePageTypeUpdation", "false");

            PageTypeBuilderConfiguration configuration = PageTypeBuilderConfiguration.GetConfiguration();

            Assert.Equal(false, configuration.DisablePageTypeUpdation);
        }

        [Fact]
        public void GivenPageTypeSyncFalseInAppConfig_DisablePageTypeUpdation_ReturnsFalse()
        {
            RecreateConfigSectionWithSingleAttribute("disablePageTypeSync", "false");

            PageTypeBuilderConfiguration configuration = PageTypeBuilderConfiguration.GetConfiguration();

            Assert.Equal(false, configuration.DisablePageTypeUpdation);
        }

        [Fact]
        public void GivenOnlyPageTypeSyncDisabledInAppConfig_DisablePageTypeUpdation_ReturnsTrue()
        {
            RecreateConfigSectionWithSingleAttribute("disablePageTypeSync", "true");

            PageTypeBuilderConfiguration configuration = PageTypeBuilderConfiguration.GetConfiguration();

            Assert.Equal(true, configuration.DisablePageTypeUpdation);
        }

        [Fact]
        public void GivenOnlyPageTypeUpdationFileEmptyInAppConfig_DisablePageTypeUpdation_ReturnsFalse()
        {
            RecreateConfigSectionWithSingleAttribute("disablePageTypeUpdationFile", "");

            PageTypeBuilderConfiguration configuration = PageTypeBuilderConfiguration.GetConfiguration();

            Assert.Equal(false, configuration.DisablePageTypeUpdation);
        }

        [Fact]
        public void GivenOnlyPageTypeSyncFileEmptyInAppConfig_DisablePageTypeUpdation_ReturnsFalse()
        {
            RecreateConfigSectionWithSingleAttribute("disablePageTypeSyncFile", "");

            PageTypeBuilderConfiguration configuration = PageTypeBuilderConfiguration.GetConfiguration();

            Assert.Equal(false, configuration.DisablePageTypeUpdation);
        }

        [Fact]
        public void GivenOnlyPageTypeUpdationFileWithNonExistingFileInAppConfig_DisablePageTypeUpdation_ReturnsFalse()
        {
            RecreateConfigSectionWithSingleAttribute("disablePageTypeUpdationFile", disableSyncFile);
            RemoveSyncDisablingFile();

            PageTypeBuilderConfiguration configuration = PageTypeBuilderConfiguration.GetConfiguration();

            Assert.Equal(false, configuration.DisablePageTypeUpdation);
        }

        [Fact]
        public void GivenOnlyPageTypeSyncFileWithNonExistingFileInAppConfig_DisablePageTypeUpdation_ReturnsFalse()
        {
            RecreateConfigSectionWithSingleAttribute("disablePageTypeSyncFile", disableSyncFile);
            RemoveSyncDisablingFile();

            PageTypeBuilderConfiguration configuration = PageTypeBuilderConfiguration.GetConfiguration();

            Assert.Equal(false, configuration.DisablePageTypeUpdation);
        }
        
        [Fact]
        public void GivenOnlyPageTypeUpdationFileWithExistingFileInAppConfig_DisablePageTypeUpdation_ReturnsTrue()
        {
            RecreateConfigSectionWithSingleAttribute("disablePageTypeUpdationFile", disableSyncFile);
            CreateSyncDisablingFile();

            PageTypeBuilderConfiguration configuration = PageTypeBuilderConfiguration.GetConfiguration();

            Assert.Equal(true, configuration.DisablePageTypeUpdation);
        }

        [Fact]
        public void GivenOnlyPageTypeSyncFileWithExistingFileInAppConfig_DisablePageTypeUpdation_ReturnsTrue()
        {
            RecreateConfigSectionWithSingleAttribute("disablePageTypeSyncFile", disableSyncFile);
            CreateSyncDisablingFile();

            PageTypeBuilderConfiguration configuration = PageTypeBuilderConfiguration.GetConfiguration();

            Assert.Equal(true, configuration.DisablePageTypeUpdation);
        }

        private void RecreateConfigSectionWithSingleAttribute(string attributeName, string attributeValue)
        {
            var doc = XDocument.Load(assemblyConfigFile, LoadOptions.PreserveWhitespace);
            var ptb = doc.Descendants("pageTypeBuilder").First();

            ptb.Attributes().Remove();
            ptb.SetAttributeValue(attributeName, attributeValue);

            doc.Save(assemblyConfigFile);
            ConfigurationManager.RefreshSection("pageTypeBuilder");
        }

        private void CreateSyncDisablingFile()
        {
            if (!File.Exists(disableSyncFile))
            {
                File.WriteAllText(disableSyncFile, "");
            }
        }

        private void RemoveSyncDisablingFile()
        {
            File.Delete(disableSyncFile);
        }
    }
}
