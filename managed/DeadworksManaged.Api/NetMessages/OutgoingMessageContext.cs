using Google.Protobuf;

namespace DeadworksManaged.Api;

/// <summary>
/// Carries a server→client net message along with its destination recipients.
/// Passed to handlers registered via <see cref="NetMessages.HookOutgoing{T}"/>.
/// </summary>
/// <typeparam name="T">The protobuf message type.</typeparam>
public class OutgoingMessageContext<T> where T : IMessage<T>
{
	/// <summary>The protobuf message being sent.</summary>
	public T Message { get; }

	/// <summary>The set of players this message will be delivered to. Modifiable by the hook handler.</summary>
	public RecipientFilter Recipients { get; set; }

	/// <summary>The numeric network message ID for <typeparamref name="T"/>.</summary>
	public int MessageId { get; }

	internal OutgoingMessageContext(T message, int messageId, RecipientFilter recipients)
	{
		Message = message;
		MessageId = messageId;
		Recipients = recipients;
	}
}
