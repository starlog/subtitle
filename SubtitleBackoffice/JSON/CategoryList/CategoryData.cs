using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SubtitleBackoffice.JSON.CategoryList
{
    public class CategoryData
    {
        public string Id;
        public string Name;
        public bool IsLeaf;

        public List<CategoryInfo> SubMenus = new List<CategoryInfo>();
    }
}