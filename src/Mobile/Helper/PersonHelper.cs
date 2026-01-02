namespace Mobile;

#region Usings
using Common;
#endregion

/// <summary>
/// Aim: Helper methods for person-related operations.
/// </summary>
public static class PersonHelper
{
	/// <summary>
	/// Aim: Gets display name from first and last name based on direction setting.
	/// Params: firstName - First name, lastName - Last name, direction - Name direction setting
	/// Return: Formatted display name
	/// </summary>
	public static string GetDisplayName(string firstName, string lastName, PersonNameDirection direction)
	{
		string result = string.Empty;

		bool tooLong = firstName.Length + lastName.Length > CommonConstants.MAX_LENGTH_DISPLAYNAME;

        switch (direction)
		{
			case PersonNameDirection.FirstFirstName:
                if (tooLong)
                {
					int maxLengthLastName = CommonConstants.MAX_LENGTH_DISPLAYNAME - firstName.Length;
					if (lastName.Contains("-"))
					{
						string[] pieces = lastName.Split('-');
						if (pieces[0].Length + 2 < maxLengthLastName)
							lastName = pieces[0] + "-" + pieces[1].Substring(0, 1) + ".";
						else
							lastName = pieces[0].Substring(0, 1) + ".";
                    }
					else
						lastName = lastName.Substring(0, maxLengthLastName - 1) + ".";
                }
                
				result = $"{firstName} {lastName}".Trim();
				
				break;

			case PersonNameDirection.FirstLastName:
				if (tooLong)
				{
					firstName = firstName.Substring(0, 1) + ".";
				}

				result = $"{lastName}, {firstName}".Trim();
				
				break;
		}		

		return result;
	}
}
