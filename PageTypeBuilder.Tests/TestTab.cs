using EPiServer.Security;

namespace PageTypeBuilder.Tests
{
    public class TestTab : Tab
    {
        private string _name;

        public override string Name
        {
            get
            {
                if(_name == null)
                    _name = TestValueUtility.CreateRandomString();

                return (_name);
            }
        }

        public override AccessLevel RequiredAccess
        {
            get { return AccessLevel.Administer; }
        }

        public override int SortIndex
        {
            get { return 1; }
        }
    }
}