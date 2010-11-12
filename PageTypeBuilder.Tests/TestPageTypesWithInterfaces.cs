using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PageTypeBuilder.Tests
{
    public static class TestEditCaptions
    {
        public const string FromInterfaceA = "FromInterface";
        public const string FromPageType = "FromPageType";
        public const string FromClashingInterface = "FromClashingInterface";

    }

    [PageType]
    public class TestPageTypeWithInterface: TypedPageData, TestInterfaceA
    {
        public virtual string InterfaceAPropertyString { get; set; }
    }

    public interface TestInterfaceA
    {
        [PageTypeProperty(EditCaption = TestEditCaptions.FromInterfaceA)]
        string InterfaceAPropertyString { get; set; }
    }

    public interface TestInterfaceClashingWithInterfaceA
    {
        [PageTypeProperty(EditCaption = TestEditCaptions.FromClashingInterface)]
        string InterfaceAPropertyString { get; set; }
    }

    [PageType]
    public class TestPageTypeWithClashingInterfaces : TypedPageData, TestInterfaceA, TestInterfaceClashingWithInterfaceA
    {
        public virtual string InterfaceAPropertyString { get; set; }
    }

    [PageType]
    public class TestPageTypeWithClashingInterfacesWhichAlsoDefinesProperty : TypedPageData, TestInterfaceA, TestInterfaceClashingWithInterfaceA
    {
        [PageTypeProperty(EditCaption = TestEditCaptions.FromPageType)]
        public virtual string InterfaceAPropertyString { get; set; }
    }

    [PageType]
    public class TestPageTypeWithInterfaceWhichAlsoDefinesProperty : TypedPageData, TestInterfaceA
    {
        [PageTypeProperty(EditCaption = TestEditCaptions.FromPageType)]
        public virtual string InterfaceAPropertyString { get; set; }
    }

}
