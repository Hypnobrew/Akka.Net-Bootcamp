using System;
using Akka.Actor;

namespace WinTail
{
    public class ValidationActor : UntypedActor
    {
        private readonly IActorRef _consoleWriterActor;
        public ValidationActor(IActorRef consoleWriterActor)
        {
            _consoleWriterActor = consoleWriterActor;
        }

        protected override void OnReceive(object message)
        {
            var msg = message as string;
            if (string.IsNullOrEmpty(msg))
            {
                _consoleWriterActor.Tell(new NullInputError("No input received."));
                Sender.Tell(new ContinueProcessing());
                return;
            }

            var valid = IsValid(msg);
            if (valid)
            {
                _consoleWriterActor.Tell(new InputSuccess("Thank you! Message was valid."));
            }
            else
            {
                _consoleWriterActor.Tell(new ValidationError("Invalid: input had odd number of characters."));
            }

            Sender.Tell(new ContinueProcessing());
        }

        private static bool IsValid(string msg)
        {
            var valid = msg.Length % 2 == 0;
            return valid;
        }
    }
}
