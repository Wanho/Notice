using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Test.Core
{
	internal sealed class CommandItem : ValueObject
	{
		private readonly static char splitor;

		private readonly static Regex VariableParameterMatcher;

		private readonly static Regex MatchParameterMatcher;

		private readonly static Regex StaticParameterMatcher;

		internal MatchCollection matchMatches;

		internal string Id
		{
			get;
			set;
		}

		internal List<MatchItem> Matches { get; private set; } = new List<MatchItem>();

		internal string Provider
		{
			get;
			set;
		}

		internal CommandResult ResultType
		{
			get;
			set;
		}

		internal string Text
		{
			get;
			set;
		}

		internal short Timeout
		{
			get;
			set;
		}

		internal CommandType Type
		{
			get;
			set;
		}

		internal CommandVerb Verb
		{
			get;
			set;
		}

		static CommandItem()
		{
			CommandItem.splitor = '｜';
			CommandItem.VariableParameterMatcher = new Regex("#\\{ *[a-zA-Z0-9ㄱ-힣_]+ *\\}", RegexOptions.Compiled);
			CommandItem.MatchParameterMatcher = new Regex("\\$\\{ *[a-zA-Z0-9ㄱ-힣_]+ *\\}", RegexOptions.Compiled);
			CommandItem.StaticParameterMatcher = new Regex("%\\{ *[a-zA-Z0-9ㄱ-힣_]+ *\\}", RegexOptions.Compiled);
		}

		public CommandItem()
		{
		}

		internal string GetParameterMatchText(List<ParameterSource> parameterSource)
		{
            CommandItem.MatchParameterMatcher.Matches(this.Text);

            return parameterSource.ToString();
            //parameterSource.Find((ParameterSource p) => p.Name.MatchDataName(expressionToken4.Name));

        }

		internal void Parse()
		{
			this.matchMatches = CommandItem.MatchParameterMatcher.Matches(this.Text);
		}

		public override string ToString()
		{
			return base.ToString();
		}
	}
}