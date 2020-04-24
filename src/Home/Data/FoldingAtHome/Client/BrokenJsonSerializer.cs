using System;
using System.Text.Json;

namespace Home.Data.FoldingAtHome.Client
{
    public class BrokenJsonSerializer
    {
        public static Slot[] DeserializeFromBrokenJson(string jsonContent)
        {
            var text = jsonContent;
            text = text.Substring(text.IndexOf(",") + 1);
            text = text.Substring(0, text.LastIndexOf("]"));
            text = text.Substring(0, text.LastIndexOf("]"));
            return JsonSerializer.Deserialize<Slot[]>(text) ?? Array.Empty<Slot>();
        }
    }
}
