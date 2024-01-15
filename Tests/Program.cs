using SharedClass.ClientObjects;
using System.ComponentModel.DataAnnotations;

var xd = new UserNameQuestionary();
xd.value = "a";

ValidationContext context = new ValidationContext(xd);
List<ValidationResult> validationResults = new List<ValidationResult>();
bool valid = Validator.TryValidateObject(xd, context, validationResults, true);
Console.WriteLine(valid);