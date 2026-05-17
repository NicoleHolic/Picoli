namespace Domain;

public static class Constants
{
    public static class Hotspot
    {
        public const string NAME = "Picoli Pi";
        public const string PASSWORD = "11112222";
    }
    
    public static class Mqtt
    {
        public const string IP = "127.0.0.1";
        public const int PORT = 1883;
    }
    
    public static class Messages
    {
        public const string MESSAGE_TOPIC = "app/message";
        public const string CONFIGURATION_TOPIC = "app/configuration";
        
        public const string RECEIVER_ALL = "*";
        public const string RECEIVER_SERVER = "SERVER";
    }
}