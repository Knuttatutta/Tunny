using System.Collections.Generic;
using System.Text;

using GH_IO.Serialization;

using Grasshopper.Kernel.Types;

using Rhino.Geometry;

using Tunny.Util;

namespace Tunny.Type
{
    public class GH_Fish : GH_Goo<Fish>
    {
        public GH_Fish()
        {
        }

        public GH_Fish(Fish fish)
            : base(fish)
        {
        }

        public GH_Fish(GH_Fish other)
            : base(other.Value)
        {
        }

        public override bool IsValid => true;
        public override string TypeName => "Fish";
        public override string TypeDescription => "Tunny optimizer result.";
        public override bool CastFrom(object source) => false;
        public override IGH_GooProxy EmitProxy() => new GH_FishProxy(this);
        public override IGH_Goo Duplicate() => new GH_Fish(this);
        public override bool Read(GH_IReader reader)
        {
            string base64 = string.Empty;
            if (reader.TryGetString("Fish_bin", ref base64))
            {
                Value = Fish.FromBase64(base64);
            }
            return base.Read(reader);
        }

        public override bool Write(GH_IWriter writer)
        {
            writer.SetString("Fish_bin", Fish.ToBase64(Value));
            return base.Write(writer);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            SetModelNumber(sb);
            SetVariables(sb);
            SetObjectives(sb);
            SetAttributes(sb);
            return sb.ToString();
        }

        private void SetAttributes(StringBuilder sb)
        {
            sb.AppendLine("------------------------------------");
            sb.AppendLine("Attributes:");
            sb.AppendLine("------------------------------------");

            foreach (KeyValuePair<string, object> attr in m_value.Attributes)
            {
                SetAttributeEachItem(sb, attr);
            }
        }

        private void SetAttributeEachItem(StringBuilder sb, KeyValuePair<string, object> attr)
        {
            var valueStrings = new StringBuilder();
            if (attr.Key == "Geometry")
            {
                List<GeometryBase> geometries = Value.GetGeometries();
                int maxLength = geometries.Count > 5 ? 5 : geometries.Count;
                for (int i = 0; i < maxLength; i++)
                {
                    string geomString = Converter.GeometryBaseToGoo(geometries[i]).ToString();
                    valueStrings.Append("\n    ");
                    valueStrings.Append(geomString);
                }
                if (geometries.Count > 5)
                {
                    valueStrings.Append(".....");
                }
            }
            else
            {
                var values = attr.Value as List<string>;
                foreach (string val in values)
                {
                    valueStrings.Append(val);
                    valueStrings.Append(", ");
                }
            }
            sb.AppendLine("  " + attr.Key + ": " + valueStrings);
        }

        private void SetObjectives(StringBuilder sb)
        {
            sb.AppendLine("------------------------------------");
            sb.AppendLine("Objectives:");
            sb.AppendLine("------------------------------------");
            if (m_value.Objectives != null)
            {
                foreach (KeyValuePair<string, double> objective in m_value.Objectives)
                {
                    sb.AppendLine("  \"" + objective.Key + "\": " + objective.Value);
                }
            }
        }

        private void SetModelNumber(StringBuilder sb)
        {
            sb.AppendLine("====================================");
            sb.AppendLine("Model Number: " + m_value.ModelNumber + "");
            sb.AppendLine("====================================");
        }

        private void SetVariables(StringBuilder sb)
        {
            sb.AppendLine("Variables:");
            sb.AppendLine("------------------------------------");
            foreach (KeyValuePair<string, double> variable in m_value.Variables)
            {
                sb.AppendLine("  \"" + variable.Key + "\": " + variable.Value + "");
            }
        }

        public class GH_FishProxy : GH_GooProxy<GH_Fish>
        {
            public GH_FishProxy(GH_Fish value)
                : base(value)
            {
            }
        }
    }
}
