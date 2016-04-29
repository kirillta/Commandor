using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace FloxDc.Commandor
{
    public class Commandor
    {
        public ILookup<string, string> Parse(string[] args)
        {
            if (args.Length == 0)
                return ToLookup(new List<KeyValuePair<string, string>>());

            var trimmed = GetArgsTrimmed(args);

            var result = new List<KeyValuePair<string, string>>();
            var command = GetCommand(trimmed[0]);

            if (trimmed.Length == 1)
            {
                result.Add(new KeyValuePair<string, string>(command, string.Empty));
                return ToLookup(result);
            }

            var normalized = NormalizeInputArray(trimmed);
            var arguments = new List<string>();

            for (var i = 1; i < normalized.Length; i++)
            {
                var value = normalized[i];

                if (!IsCommand(value))
                {
                    arguments.Add(value);
                    if (i != normalized.Length -1)
                        continue;
                }

                AddArguments(command, arguments, result);
                arguments = new List<string>();
                command = GetCommand(value);
            }

            return ToLookup(result);
        }


        private List<KeyValuePair<string, string>> AddArguments(string command, List<string> arguments, List<KeyValuePair<string, string>> target)
        {
            if (arguments.Any())
                target.AddRange(arguments.Select(arg => new KeyValuePair<string, string>(command, arg)));
            else
                target.Add(new KeyValuePair<string, string>(command, string.Empty));

            return target;
        } 


        private string GetCommand(string target)
            => Regex.Match(target, @"^(-{1,2}|/)(?<command>\w+)(:|=)?.*").Groups["command"].Value.ToLower();


        private bool IsCommand(string target)
            => Regex.IsMatch(target, @"^(-{1,2}|/).*");


        private string[] GetArgsTrimmed(string[] args)
        {
            while (args.Length > 0 && !IsCommand(args[0]))
                args = args.Skip(1).ToArray();

            return args;
        }


        private string[] NormalizeInputArray(string[] target)
        {
            var commandArgRegex = @"^(?<command>(-{1,2}|/)\w+)(:|=){1}(?<arg>.*)";
            var result = new List<string>();

            foreach (var str in target)
            {
                var s = str;

                if (IsCommand(s))
                {
                    if (!Regex.IsMatch(s, commandArgRegex))
                    {
                        result.Add(s);
                        continue;
                    }

                    var ms = Regex.Match(s, commandArgRegex);

                    result.Add(ms.Groups["command"].Value);
                    if (!ms.Groups["arg"].Success)
                        continue;

                    s = ms.Groups["arg"].Value;
                }

                if (s.StartsWith("\"") && s.EndsWith("\""))
                    s = s.Substring(1, s.Length - 2);

                result.Add(s);
            }

            return result.ToArray();
        }


        private ILookup<string, string> ToLookup(List<KeyValuePair<string, string>> source) 
            => source.ToLookup(s => s.Key, s => s.Value);
    }
}
