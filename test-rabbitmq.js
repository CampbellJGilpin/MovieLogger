const amqp = require('amqplib');

async function testRabbitMQ() {
    try {
        // Connect to RabbitMQ
        const connection = await amqp.connect('amqp://movieuser:hotrod1@localhost:5672');
        const channel = await connection.createChannel();

        // Declare the exchange
        await channel.assertExchange('movie_events', 'topic', { durable: true });

        // Test message that matches the MovieAddedEvent structure
        const testMessage = {
            movieId: 10, // Ghostbusters ID
            movieTitle: "Ghostbusters",
            genre: "Comedy",
            eventType: "MovieAdded",
            timestamp: new Date().toISOString(),
            userId: "test-user"
        };

        console.log('Sending test message:', JSON.stringify(testMessage, null, 2));

        // Publish the message
        await channel.publish('movie_events', 'movieadded.test-user', Buffer.from(JSON.stringify(testMessage)));

        console.log('Test message sent successfully!');

        // Close connection
        await channel.close();
        await connection.close();

        console.log('Connection closed.');
    } catch (error) {
        console.error('Error:', error);
    }
}

testRabbitMQ(); 