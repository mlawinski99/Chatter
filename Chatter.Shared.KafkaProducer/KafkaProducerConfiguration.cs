namespace Chatter.Shared.KafkaProducer;

public class KafkaProducerConfiguration
{
        public string BootstrapServers { get; set; }
        public bool EnableIdempotence { get; set; }
        public int MessageTimeoutMs { get; set; }
        public string Acks { get; set; }
}