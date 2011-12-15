using System;
using EPiServer.Core;
using EPiServer.Core.PropertySettings;
using EPiServer.DataAbstraction;
using EPiServer.Editor;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Discovery;
using PageTypeBuilder.Synchronization.PageDefinitionSynchronization;
using Rhino.Mocks;
using Xunit;

namespace PageTypeBuilder.Tests.Synchronization.PageDefinitionSynchronizationEngineTests
{
	public class PageDefinitionSpecificPropertySettingsUpdaterTests
	{
		[Fact]
		public void GivenPagePropertyWithSettings_AndPropertyIsProtected_WillFindPagePropertyAndSaveSettings()
		{
			VerifyPagePropertyIsFoundAndSettingsAreSaved(TestPageType.ProtectedPropertyName);
		}

		[Fact]
		public void GivenPagePropertyWithSettings_AndPropertyIsPublic_WillFindPagePropertyAndSaveSettings()
		{
			VerifyPagePropertyIsFoundAndSettingsAreSaved(TestPageType.PublicPropertyName);
		}

		private static void VerifyPagePropertyIsFoundAndSettingsAreSaved(string propertyName)
		{
			var fakes = new MockRepository();
			var propertySettingsRepository = fakes.DynamicMock<IPropertySettingsRepository>();
			propertySettingsRepository.Expect(x => x.Save(null)).IgnoreArguments();
			propertySettingsRepository.Replay();
			var pageDefinitionRepository = fakes.DynamicMock<IPageDefinitionRepository>();
			var updater = new PageDefinitionSpecificPropertySettingsUpdater(propertySettingsRepository,
			    null /* unused, but could be a dynamic mock if null checks are added */,
			    pageDefinitionRepository);
			var pageTypeDefinition = CreatePageTypeDefinition();
			var propertyDefinition = CreatePageTypePropertyDefinition(propertyName);

			updater.UpdatePropertySettings(pageTypeDefinition, propertyDefinition, new PageDefinition());

			propertySettingsRepository.VerifyAllExpectations();
		}

		[Fact]
		public void GivenPagePropertyWithSettings_AndPropertyIsMissingOnPageType_WillThrowPageTypeBuilderException()
		{
			var updater = new PageDefinitionSpecificPropertySettingsUpdater(null, null, null);
			var pageTypeDefinition = CreatePageTypeDefinition();
			var propertyDefinition = CreatePageTypePropertyDefinition(TestPageType.InvalidPropertyName);

			var ex = Assert.Throws<PageTypeBuilderException>(() =>
				updater.UpdatePropertySettings(pageTypeDefinition, propertyDefinition, new PageDefinition())
			);
			Assert.Equal("Unable to locate the property \"" + TestPageType.InvalidPropertyName + "\" in PageType \"TestPage\".", ex.Message);
		}

		private static PageTypeDefinition CreatePageTypeDefinition()
		{
			return new PageTypeDefinition
			{
				Type = typeof (TestPageType),
				Attribute = new PageTypeAttribute
				{
					Name = "TestPage"
				}
			};
		}

		private static PageTypePropertyDefinition CreatePageTypePropertyDefinition(string name)
		{
			Type type = typeof(string);
			IPageType pageType = new TestPageType();
			var attribute = new PageTypePropertyAttribute
			{
				SortOrder = 1,
				Required = false,
				Type = typeof(PropertyLongString),
				EditCaption = name
			};
			return new PageTypePropertyDefinition(name, type, pageType, attribute);
		}

		public class TestPageType : NativePageType
		{
			public const string ProtectedPropertyName = "ProtectedProperty";
			public const string PublicPropertyName = "PublicProperty";
			public const string InvalidPropertyName = "NoSuchProperty";

			[PageTypeProperty(SortOrder = 1, Required = false, Type = typeof(PropertyLongString), EditCaption = ProtectedPropertyName)]
			[TestSettingsAttribute]
			protected string ProtectedProperty { get; set; }

			[PageTypeProperty(SortOrder = 2, Required = false, Type = typeof(PropertyLongString), EditCaption = PublicPropertyName)]
			[TestSettingsAttribute]
			public string PublicProperty { get; set; }
		}

		[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
		public sealed class TestSettingsAttribute : Attribute, IUpdatePropertySettings<HtmlEditorSettings>
		{
			public void UpdateSettings(HtmlEditorSettings settings)
			{
				settings.Value = 0;
			}

			public int GetSettingsHashCode(HtmlEditorSettings settings)
			{
				return 12321; // Will trigger updated/save
			}

			public bool OverWriteExistingSettings
			{
				get { return true; }
			}
		}
	}
}