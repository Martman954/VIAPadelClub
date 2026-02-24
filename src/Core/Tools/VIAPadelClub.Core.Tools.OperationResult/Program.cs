using VIAPadelClub.Core.Tools.OperationResult.Entities;
using VIAPadelClub.Core.Tools.OperationResult.Results.Errors;

namespace VIAPadelClub.Core.Tools.OperationResult;

/// <summary>
/// Program for Testing and Simulation.
/// </summary>
public class Program
{
    public static void Main(string[] args)
    {
        

        Console.WriteLine("=== SCENARIO 1: TOTAL FAILURE (Multiple Errors) ===");
        var totalFail = Player.Register("", " ", "123", "");
        PrintResult(totalFail);

        Console.WriteLine("\n=== SCENARIO 2: INCORRECT EMAIL (F1/F2 Rules) ===");
        var emailFail = Player.Register("trmo@gmail.com", "Troels", "Mortensen", "http://via.dk/pic.jpg");
        PrintResult(emailFail);

        Console.WriteLine("\n=== SCENARIO 3: SUCCESS & FORMATTING (S1 Rule) ===");
        var success = Player.Register("123456@VIA.DK", "tROELS", "mORTENSEN", "http://via.dk/pic.jpg");
        PrintResult(success);
    }

    private static void PrintResult(Result<Player> result)
    {
        if (result is Result<Player>.Success success)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("✔ SUCCESS!");
            Console.WriteLine($"   Id: {success.Value.Id}");
            Console.WriteLine($"   Name: {success.Value.Firstname} {success.Value.Lastname}");
            Console.WriteLine($"   Email: {success.Value.Email}");
            Console.ResetColor();
        }
        else if (result is Result<Player>.Failure failure)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            foreach (var error in failure.Errors)
            {
                Console.WriteLine($"✘ {error.Message}");
            }
            Console.ResetColor();
        }
    }
}