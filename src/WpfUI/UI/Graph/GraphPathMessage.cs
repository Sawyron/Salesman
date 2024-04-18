using CommunityToolkit.Mvvm.Messaging.Messages;
using Salesman.Domain.Graph;

namespace WpfUI.UI.Graph;
public class GraphPathMessage(PathResult<int, int> pathResult)
    : ValueChangedMessage<PathResult<int, int>>(pathResult);
