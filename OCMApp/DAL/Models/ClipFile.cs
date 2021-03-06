﻿using OCMClip.ClipHandler.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCMApp.DAL.Models
{
    public class ClipFile : Clip
    {
        public string Preview { get; set; }
        public string Value { get; set; }

        public ClipFile()
        {

        }

        public ClipFile(ClipDataFile entity) : this()
        {
            if (entity.Value != null)
            {
                Value = "";
                for (int i = 0; i < entity.Value.Count; i++)
                {
                    if (i == 0)
                        Preview = entity.Value[i];
                    Value += entity.Value[i];
                    if (i < (entity.Value.Count - 1))
                        Value += ";";
                }
            }
            base.Set(entity);
        }

        public List<string> GetListValue()
        {
            List<string> retVal = new List<string>();
            if (!string.IsNullOrWhiteSpace(Value))
            {
                foreach (var item in Value.Split(';'))
                {
                    retVal.Add(item.Trim());
                }
            }
            return retVal;
        }
    }
}
