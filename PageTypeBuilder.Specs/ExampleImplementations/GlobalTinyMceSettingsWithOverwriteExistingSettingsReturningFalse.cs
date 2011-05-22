namespace PageTypeBuilder.Specs.ExampleImplementations
{
    public class GlobalTinyMceSettingsWithOverwriteExistingSettingsReturningFalse : GlobalTinyMceSettings
    {
        public override string DisplayName
        {
            get
            {
                return base.DisplayName + "with OverwriteExistingSettings returning false";
            }
        }
        public override bool OverWriteExistingSettings
        {
            get
            {
                return false;
            }
        }
    }
}