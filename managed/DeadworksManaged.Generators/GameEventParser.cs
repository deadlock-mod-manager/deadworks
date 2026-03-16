using System.Collections.Generic;

namespace DeadworksManaged.Generators;

internal readonly struct GameEventField
{
    public readonly string Name;
    public readonly string Type;

    public GameEventField(string name, string type)
    {
        Name = name;
        Type = type;
    }
}

internal readonly struct GameEventDefinition
{
    public readonly string Name;
    public readonly List<GameEventField> Fields;

    public GameEventDefinition(string name, List<GameEventField> fields)
    {
        Name = name;
        Fields = fields;
    }
}

internal static class GameEventParser
{
    private static readonly HashSet<string> MetadataKeys = new HashSet<string>
    {
        "local", "reliable"
    };

    private static readonly HashSet<string> SkipTypes = new HashSet<string>
    {
        "none", "local"
    };

    public static List<GameEventDefinition> Parse(string text)
    {
        var events = new List<GameEventDefinition>();
        var lines = text.Split(new[] { '\r', '\n' });

        int i = 0;
        int braceDepth = 0;
        string currentEventName = null;
        var currentFields = new List<GameEventField>();
        bool insideTopLevel = false;

        while (i < lines.Length)
        {
            var line = lines[i].Trim();
            i++;

            // Skip empty lines and comments
            if (line.Length == 0 || line.StartsWith("//"))
                continue;

            // Count braces
            if (line == "{")
            {
                braceDepth++;
                if (braceDepth == 1)
                {
                    // Top-level brace (the "gameevents" block)
                    insideTopLevel = true;
                }
                else if (braceDepth == 2 && currentEventName != null)
                {
                    // Event body starts
                }
                continue;
            }

            if (line == "}")
            {
                if (braceDepth == 2 && currentEventName != null)
                {
                    // Event body ends
                    events.Add(new GameEventDefinition(currentEventName, currentFields));
                    currentEventName = null;
                    currentFields = new List<GameEventField>();
                }
                braceDepth--;
                if (braceDepth == 0)
                    insideTopLevel = false;
                continue;
            }

            if (!insideTopLevel)
                continue;

            // Parse quoted strings
            var tokens = ParseQuotedTokens(line);

            if (braceDepth == 1)
            {
                // Event name line - could be: "event_name" or "event_name" { or "event_name" {}
                if (tokens.Count >= 1)
                {
                    var name = tokens[0];
                    // Check for inline {} on same line
                    var rest = line.Substring(line.IndexOf('"') + name.Length + 1).Trim();

                    if (rest == "{}" || rest.StartsWith("{}"))
                    {
                        // Empty event, add directly
                        events.Add(new GameEventDefinition(name, new List<GameEventField>()));
                    }
                    else if (rest == "{")
                    {
                        // Brace on same line after event name
                        braceDepth++;
                        currentEventName = name;
                        currentFields = new List<GameEventField>();
                    }
                    else
                    {
                        // Event name only, brace will come on next line
                        currentEventName = name;
                        currentFields = new List<GameEventField>();
                    }
                }
            }
            else if (braceDepth == 2 && currentEventName != null)
            {
                // Field line: "key" "type"
                if (tokens.Count >= 2)
                {
                    var fieldName = tokens[0];
                    var fieldType = tokens[1];

                    // Skip metadata entries
                    if (MetadataKeys.Contains(fieldName))
                        continue;

                    // Skip fields with metadata/skip types
                    if (SkipTypes.Contains(fieldType))
                        continue;

                    // Skip values like "1" or "0" that are metadata
                    if (fieldType == "1" || fieldType == "0")
                        continue;

                    currentFields.Add(new GameEventField(fieldName, fieldType));
                }
            }
        }

        return events;
    }

    private static List<string> ParseQuotedTokens(string line)
    {
        var tokens = new List<string>();
        int pos = 0;

        while (pos < line.Length)
        {
            // Find opening quote
            int start = line.IndexOf('"', pos);
            if (start < 0) break;

            // Find closing quote
            int end = line.IndexOf('"', start + 1);
            if (end < 0) break;

            tokens.Add(line.Substring(start + 1, end - start - 1));
            pos = end + 1;
        }

        return tokens;
    }
}
