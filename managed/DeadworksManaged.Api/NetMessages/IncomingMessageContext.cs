using Google.Protobuf;

namespace DeadworksManaged.Api;

/// <summary>
/// Carries a client→server net message along with its sender.
/// Passed to handlers registered via <see cref="NetMessages.HookIncoming{T}"/>.
/// </summary>
/// <typeparam name="T">The protobuf message type.</typeparam>
public class IncomingMessageContext<T> where T : IMessage<T>
{
	/// <summary>The protobuf message received from the client.</summary>
	public T Message { get; }

	/// <summary>The player slot index of the client that sent this message.</summary>
	public int SenderSlot { get; }

	/// <summary>The numeric network message ID for <typeparamref name="T"/>.</summary>
	public int MessageId { get; }

	internal IncomingMessageContext(T message, int messageId, int senderSlot)
	{
		Message = message;
		MessageId = messageId;
		SenderSlot = senderSlot;
	}
}
