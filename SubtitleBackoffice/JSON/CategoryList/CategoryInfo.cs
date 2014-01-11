using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SubtitleBackoffice.JSON.CategoryList
{
    public class CategoryInfo
    {
        public string Id;
        public string SubtitleId;
        public string Name;
        public bool IsLeaf;
        public string Pooq_ID;

        public CategoryInfo(string _id, string _name, bool _isLeaf, string _pooq_id)
        {
            this.Id = _pooq_id;
            this.SubtitleId = _id;
            this.Name = _name;
            this.IsLeaf = _isLeaf;
            this.Pooq_ID = _pooq_id;
        }
    }
}