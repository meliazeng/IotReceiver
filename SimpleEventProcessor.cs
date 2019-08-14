using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;
using System.Threading.Tasks;
using Cassandra;
using Cassandra.Mapping;


namespace receiverApp
{
    public class SimpleEventProcessor : IEventProcessor
    {
	// Cassandra Cluster Configs
	private const string UserName = "";
	private const string Password = "";
	private const string CassandraContactPoint = ""; //DNSName
	private static int CassandraPort = 9042;
    
        public Task CloseAsync(PartitionContext context, CloseReason reason)
	{
            Console.WriteLine($"Processor Shutting Down. Partition '{context.PartitionId}', Reason: '{reason}'.");
	        return Task.CompletedTask;
        }

        public Task OpenAsync(PartitionContext context)
        {
            Console.WriteLine($"SimpleEventProcessor initialized. Partition: '{context.PartitionId}'");
	        return Task.CompletedTask;
        }

	public Task ProcessErrorAsync(PartitionContext context, Exception error)
        {
            Console.WriteLine($"Error on Partition: {context.PartitionId}, Error: {error.Message}");
                return Task.CompletedTask;
        }

        public Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
        {
            foreach (var eventData in messages)
            {
                var data = Encoding.UTF8.GetString(eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);
                // todo:: make the save to database work.
		Cluster cluster = Cluster.Builder().WithCredentials(UserName, Password).WithPort(CassandraPort).AddContactPoint(CassandraContactPoint).Build();
		ISession session = cluster.Connect();

		session.Execute("DROP KEYSPACE IF EXISTS uprofile");
		session.Execute("CREATE KEYSPACE uprofile WITH REPLICATION = { 'class': 'NetworkTopologyStrategy', 'datacenter1': 1};");
		session.Execute("CREATE TABLE IF NOT EXISTS uprofile.user (user_id int PRIMARY KEY, user_name text, user_bcity text)");
                session = cluster.Connect("uprofile");
		IMapper mapper = new Mapper(session);

		mapper.Insert<SData>(new SData(data, DateTime.Now, 0));


		Console.WriteLine($"Message received. Partition: '{context.PartitionId}', Data: '{data}'");
            }
            return context.CheckpointAsync();
        }
    }
}
	      	
	


