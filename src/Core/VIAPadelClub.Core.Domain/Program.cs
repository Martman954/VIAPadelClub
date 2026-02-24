using VIAPadelClub.Core.Domain.Services;
using VIAPadelClub.Core.Tools.OperationResult.Result;

namespace VIAPadelClub.Core.Domain;

/// <summary>
/// Program for Testing and Simulation.
/// </summary>
public class Program
{
    public static void Main(string[] args)
    {
        var service = new PlayerRegistrationService();

        Console.WriteLine("=== SCENARIO 1: TOTAL FAILURE (Multiple Errors) ===");
        var totalFail = service.Register("", " ", "123", "");
        PrintResult(totalFail);

        Console.WriteLine("\n=== SCENARIO 2: INCORRECT EMAIL (F1/F2 Rules) ===");
        var emailFail = service.Register("trmo@gmail.com", "Troels", "Mortensen", "http://via.dk/pic.jpg");
        PrintResult(emailFail);

        Console.WriteLine("\n=== SCENARIO 3: SUCCESS & FORMATTING (S1 Rule) ===");
        var success = service.Register("123456@VIA.DK", "tROELS", "mORTENSEN", "http://via.dk/pic.jpg");
        PrintResult(success);
    }

    private static void PrintResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("✔ SUCCESS!");
            Console.WriteLine($"   Data: {result.Value}"); 
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("✘ FAILURE!");
            foreach (var error in result.Errors)
            {
                Console.WriteLine($"   [{error.Code}] {error.Message}");
            }
            Console.ResetColor();
        }
    }
}