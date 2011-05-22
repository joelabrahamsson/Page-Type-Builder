namespace PageTypeBuilder.Specs.ExampleImplementations
{
    public class GlobalTinyMceSettingsWithIsDefaultReturningTrue : GlobalTinyMceSettings
    {
        public override string DisplayName
        {
            get
            {
                return "Global TinyMCE settings with IsDefault returning true";
            }
        }

        public override bool? IsDefault
        {
            get
            {
                return true;
            }
            set
            {
                base.IsDefault = value;
            }
        }
    }
}