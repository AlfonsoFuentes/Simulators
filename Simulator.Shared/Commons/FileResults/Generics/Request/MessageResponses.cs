using Simulator.Shared.StaticClasses;

namespace Simulator.Shared.Commons.FileResults.Generics.Request
{
    public interface IMessageResponse
    {
        string Legend { get; }
        string ClassName { get; }
        string Succesfully { get; }
        string Fail { get; }
        string NotFound { get; }
    }
    public abstract class ValidateMessageResponse
    {
        public abstract string Legend { get; }
        public abstract string ClassName { get; }
        public string Succesfully => StaticClass.ResponseMessages.ReponseSuccesfullyMessageCreated(Legend, ClassName);
        public string Fail => StaticClass.ResponseMessages.ReponseFailMessageCreated(Legend, ClassName);


    }
    public interface ICreateMessageResponse
    {
        string Legend { get; }
        string ClassName { get; }
        string Succesfully { get; }
        string Fail { get; }
    }
    public abstract class CreateMessageResponse: ICreateMessageResponse
    {
        public abstract string Legend { get; }
        public abstract string ClassName { get; }
        public string Succesfully => StaticClass.ResponseMessages.ReponseSuccesfullyMessageCreated(Legend, ClassName);
        public string Fail => StaticClass.ResponseMessages.ReponseFailMessageCreated(Legend, ClassName);


    }
    public abstract class UpdateMessageResponse
    {
        public abstract string Legend { get; }
        public abstract string ClassName { get; }
        public string Succesfully => StaticClass.ResponseMessages.ReponseSuccesfullyMessageUpdated(Legend, ClassName);
        public string Fail => StaticClass.ResponseMessages.ReponseFailMessageUpdate(Legend, ClassName);
        public string NotFound => StaticClass.ResponseMessages.ReponseNotFound(ClassName);

    }
    public abstract class DeleteMessageResponse
    {
        public abstract string Legend { get; }
        public abstract string ClassName { get; }
        public string Succesfully => StaticClass.ResponseMessages.ReponseSuccesfullyMessageDelete(Legend, ClassName);
        public string Fail => StaticClass.ResponseMessages.ReponseFailMessageDelete(Legend, ClassName);
        public string NotFound => StaticClass.ResponseMessages.ReponseNotFound(ClassName);

    }
    public abstract class GetByIdMessageResponse
    {
        public abstract string ClassName { get; }
        public string NotFound => StaticClass.ResponseMessages.ReponseNotFound(ClassName);

    }
}
