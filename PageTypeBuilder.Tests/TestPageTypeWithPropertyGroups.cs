﻿namespace PageTypeBuilder.Tests
{
    [PageType]
    public class TestPageTypeWithPropertyGroups : TypedPageData
    {
        [PageTypeProperty(EditCaption = "Property one", SortOrder = 100)]
        public virtual string LongStringProperty { get; set; }

        [PageTypePropertyGroup(EditCaptionPrefix = "Image one - ", StartSortOrderFrom = 400)]
        public virtual Image ImageOne { get; set; }

        [PageTypePropertyGroup(EditCaptionPrefix = "Image two - ", StartSortOrderFrom = 500)]
        public virtual Image ImageTwo { get; set; }

        [PageTypePropertyGroup(EditCaptionPrefix = "Image three - ", StartSortOrderFrom = 600)]
        public virtual Image ImageThree { get; set; }
    }

    public class Image : PageTypePropertyGroup
    {
        [PageTypeProperty(EditCaption = "Image Url", SortOrder = 0)]
        public virtual string ImageUrl { get; set; }

        [PageTypeProperty(EditCaption = "Alt text", SortOrder = 10)]
        public virtual string AltText { get; set; }

        [PageTypeProperty(EditCaption = "Nullable test int property", SortOrder = 20)]
        public virtual int? NullableTestIntProperty { get; set; }

        [PageTypeProperty(EditCaption = "int test property", SortOrder = 30)]
        public virtual int IntTestProperty { get; set; }

        [PageTypeProperty(EditCaption = "string test property", SortOrder = 40)]
        public string StringTestProperty { get; set; }
        
    }

}
