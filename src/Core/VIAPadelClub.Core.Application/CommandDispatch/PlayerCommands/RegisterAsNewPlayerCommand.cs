using VIAPadelClub.Core.Domain.Aggregates.Players.ValueObjects;
using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace VIAPadelClub.Core.Application.CommandDispatch.PlayerCommands;

public class RegisterAsNewPlayerCommand
{
    public ViaEmail Email { get; set; }
    public Name Name { get; set; }
    public ImageUrl ImageUrl { get; set; }
    private RegisterAsNewPlayerCommand(ViaEmail email, Name name, ImageUrl imageUrl)
    {
        Email = email;
        Name = name;
        ImageUrl = imageUrl;
    }

    public static Result<RegisterAsNewPlayerCommand> Create(string email, string firstname, string lastname, string imageUrl)
    {
        var emailResult = ViaEmail.CreateEmail(email);
        var nameResult = Name.CreateName(firstname, lastname);
        var imageResult = ImageUrl.CreateImageUrl(imageUrl);

        return Result.CombineResultsInto<RegisterAsNewPlayerCommand>(emailResult, nameResult, imageResult)
            .WithPayloadIfSuccess(() => new RegisterAsNewPlayerCommand(emailResult.Payload, nameResult.Payload, imageResult.Payload));
    }
    
}