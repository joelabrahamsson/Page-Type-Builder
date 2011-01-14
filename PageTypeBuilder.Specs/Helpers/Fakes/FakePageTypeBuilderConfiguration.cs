using PageTypeBuilder.Configuration;

namespace PageTypeBuilder.Specs.Helpers.Fakes
{
    public class FakePageTypeBuilderConfiguration : PageTypeBuilderConfiguration
    {
        private bool disablePageTypeUpdation;

        public void SetDisablePageTypeUpdation(bool value)
        {
            disablePageTypeUpdation = value;
        }

        public override bool DisablePageTypeUpdation
        {
            get { return disablePageTypeUpdation; } 
        }
    }
}
