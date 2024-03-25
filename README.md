# Automation Testing

The source repository hosting this test is read-only.  
Before you begin, you will need to import it into your own hosted repository.  

## SQS Automation Testing

Set up an AWS SQS queue (either in a real AWS account or using the local SQS emulator).
Write an automated test script in a programming language of your choice (e.g., Python, Java, C#) to perform the following tasks:
Send a message to the SQS queue.
Receive the message from the SQS queue.
Verify that the received message matches the sent message.
Ensure that the test script handles any potential errors or exceptions gracefully.
Use appropriate AWS SDK or libraries for interacting with SQS (e.g., boto3 for Python, AWS SDK for Java, AWS SDK for .NET).
Implement assertions or validation checks to verify the correctness of the test results.

## JSON Automation Testing

There is a JSON file in the project called Cost Analysis.json
It contains an array of objects
Build a class that could be used to deserialize a single object from that array
Within JSONTest.cs

- Instantiate a list of the object you have defined
- Deserialise the json file into your list
- Newtonsoft.json has been installed for this purpose
- Write to Assert how many items are in your list

Using LINQ:

- Get the top item ordered by Cost descending, and write to Assert the CountryId.
- Sum Cost for 2016 and write to Assert the total.

## Calculator Automation Testing

Implement unit tests for a simple calculator class.
Requirements:
Create a Calculator class with methods for basic arithmetic operations: addition, subtraction, multiplication, and division.
Implement unit tests to verify the correctness of each arithmetic operation method.
Use a unit testing framework compatible with the programming language of your choice (e.g., JUnit for Java, NUnit for C#, pytest for Python).
Ensure that each method is tested for various input scenarios, including positive and negative numbers, zero, and edge cases.

## TestAutomation.cs

From the file (TestAutomation.cs) given, can you find any problems in the code?

## Finish
Please let us know where you have hosted your solution!
