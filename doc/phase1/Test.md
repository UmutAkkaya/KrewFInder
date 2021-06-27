The backend services will be tested by mocking dependent services (such as database models, endpoints) (for example, given a fake request with certain parameters, does the controller behaves as expected?)

Front end will be tested by mocking back-end responses with fake data.

Test matchmaking algorithms using native unit tests with NUnit for .Net.

The advantage of using mocks for testing is that a given component can be tested without the actual data provider (i.e. we can test front end without having complete back-end).

Once we have our test suits ready, we plan to perform continuous integration testing using Travis CI or a similar service.
