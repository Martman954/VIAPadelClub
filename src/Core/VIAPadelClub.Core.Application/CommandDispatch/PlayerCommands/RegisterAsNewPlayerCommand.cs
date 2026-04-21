using VIAPadelClub.Core.Domain.Aggregates.Player.ValueObjects;
using VIAPadelClub.Core.Domain.Aggregates.Schedule.Enums;
using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace Features.CommandDispatch.PlayerCommands;

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
        Result<ViaEmail> emailResult = ViaEmail.CreateEmail(email);
        Result<Name> nameResult = Name.CreateName(firstname, lastname);
        Result<ImageUrl> imageResult = ImageUrl.CreateImageUrl(imageUrl);

        return Result.CombineResultsInto<RegisterAsNewPlayerCommand>(emailResult, nameResult, imageResult)
            .WithPayloadIfSuccess(() => new RegisterAsNewPlayerCommand(emailResult.Payload, nameResult.Payload, imageResult.Payload));
    }
    
}