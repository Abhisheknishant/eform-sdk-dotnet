﻿using System;

namespace eFormRequest
{
    [Serializable()]
    public class KeyValuePair
    {
        #region con
        internal KeyValuePair()
        {

        }

        public KeyValuePair(string key, string value, bool selected, string displayOrder)
        {
            Key = key;
            Value = value;
            Selected = selected;
            DisplayOrder = displayOrder;
        }
        #endregion

        #region var
        public string Key { get; set; }
        public string Value { get; set; }
        public bool Selected { get; set; }
        public string DisplayOrder { get; set; }
        #endregion
    }
}