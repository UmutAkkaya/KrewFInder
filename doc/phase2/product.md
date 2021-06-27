# Project Phase 2 Report

## What we built
During this phase, we accomplished a majority of the tasks for our functional MVP, as can be seen on our Kanban board in the Github Projects page. We built the frontend and backend in isolation of each other. Our frontend consists of a React app with Redux and the backend consists of C# and MongoDB.

For the frontend side, we managed to implement and style the majority of our components, such as the homepage, login/register, group/course creation and user/group/course dashboards. We have also taken the steps to prepare our components for communication with our API once the backend is ready to service those components.

On the backend side, we have completed a basic implementation of many of the User and Group services and functionality, as well as an implementation of the JSON Web Token authentication mechanism. We also have implemented an ORM abstraction for our Mongo database, as well as a Unit Testing pipeline.

## Documents 
Over the course of this phase, we have numerous artifacts to show our progress towards the functional MVP.

- We used the Kanban development process and tracked all of our tasks in our Kanban board on Github Projects seen [here](https://github.com/csc302-winter-2018/proj-Da_Krew/projects/1)
- To adhere to the best software engineering practices and code quality, we used Github pull requests to have our work code reviewed and approved by others. Our pull requests can be seen [here](https://github.com/csc302-winter-2018/proj-Da_Krew/pulls)
- We’ve created mockups for the various pages for our frontend to give everyone a visual idea of how we want our MVP to look and feel. These mockups can be found [here](https://github.com/csc302-winter-2018/proj-Da_Krew/tree/master/doc/mockups)
- For the backend, we’ve created a file to visualize the data representation in MongoDB for the team to get a quick understanding of how everything is laid out. The database schema can be found [here](https://github.com/csc302-winter-2018/proj-Da_Krew/blob/master/doc/dbschema.md)
- Our group merging and user invitations can get messy if not understood well, so we created a document to help developers understand the flow of these, seen [here](https://github.com/csc302-winter-2018/proj-Da_Krew/blob/master/doc/status-to-action-mapping.md)
- To familiarize developers to the codebase faster and to have a robust working product, we’ve created numerous tests for our backend API, seen [here](https://github.com/csc302-winter-2018/proj-Da_Krew/tree/master/tests)

## High-level design

Our high-level design is a fairly straightforward microservice implementation featuring isolated services for Users, Groups, Courses, and Authentication built in ASP.NET Core. Each service can communicate with the central (Mongo) database through a DB abstraction layer. Our front-end is a React app, which can communicate with the different microservices as needed. Authorization and authentication is done via JSON Web Tokens, which are validated upon request and contain user roles and identifiers, allowing minimal access to the database to fetch session user info.

The React app structure is organized similar to the back-end with, routes and pages viewing organizations, groups, and users. The styling is done via a material design library, and the site is mostly responsive.

We found that there is currently no need for the microservices to communicate with each other, and adding an extra database microservice would be redundant, so none of our microservices talk to each other. Even our authentication technique allows microservices to verify user authority on their own (see JWT section).
If the need arises for inter-service communication, we have the supporting technology ready for seamless integration which allows one microservice to instantiate an interface of another microservice as a class which methods actually make web requests rather than using local implementation. For example, UserController might instantiate IGroupController using our microservice communication technology and running .GetGroup(groupId) would result in a web request to the Group microservice with serialization and deserialization abstracted away.

## Technical highlights

### ORM

One of the most difficult parts in building C# back-end was creating our own type of ORM. The reason for that is that we had some data that could be of free format and would not fit into a normal relational schema, while on the other hand, we needed strong relations between entities such as users, groups, etc.
We picked MongoDb as our back-end database, but since it is not designed to support entity relations out of the box, we had to write our own code to handle those. For example, we don’t want to load everything that a user can be related to (i.e. all courses and groups, all members of those, etc) at once, since we don’t need most of the time. In order to accomodate that, we built our own collections that support lazy loading (i.e. only query the database when we actually need it). For example, group members are not loaded until the code accesses group.Members.Value field.
As a result, this approach allows us to store items that cannot be easily stored in a relational database (such as Dictionary<User, List<Skill>> where skill is a base class for all different kinds of skills that may contain different underlying data, in a transparent and high performance way for the client code.

### React

React was an interesting challenge to try and learn. Although the paradigm of separating and isolating components is nothing new to Object Oriented Programming, React’s abstractions take a lot of getting used to. Many of our components contain far more logic and markup than they should, but as far as first tries go, at least it mostly works.

### JWT 

Although .NET supports JWT out of the box, their implementation fits in with their rather complex authorization mechanisms which currently lack documentation and examples. Thus we decided to roll our simple integration of JWT, using .NET API to create, validate, and decode the tokens (giving us the security and reliability that comes with standard APIs), but using our own mechanisms to provide the user information completely transparently to all our services. This operation gave us insight into the (newly renovated) middleware and filtering functionality of ASP.NET, and opened the door to other powerful features in the future.

### CORS

Cors has been a massive pain to work with for this entire project. Although it may become irrelevant in production where all services and frontend are hosted on the same origin, it is necessary to support it for development were microservices and frontend apps are hosted on different ports, and are considered CORS requests. It has been a very tricky thing to try to conform to browser standards while maintaining a stable set of middleware.


## Documentation
Our projects documentation can be found [here](https://github.com/csc302-winter-2018/proj-Da_Krew/blob/master/README.md)

## Strengths and weaknesses of our process
### Strengths
- Using pull requests, we adhere to the best software engineering practices and improve code quality while also reducing the chance for bugs since we have a second pair of eyes look over our work.
- The team split in half to work on the frontend and backend, so each half of the team specializes on their side of the work which improves development time significantly because there is no context switching and need to learn a whole new system.
- Using Facebook Messenger and Skype as a means of team communication because (1) you can get immediate responses for problems you’re having and (2) to get face-to-face time with the whole team to discuss progress and arising issues together in real time.
- Using the Kanban board to create tasks so others aren’t stepping on each other and wasting development time by doing duplicate work.
- Providing unit tests adds reliability to the codebase and allows simple integration of Travis CI or equivalent service to ensure that the code works before merging pull requests.


### Weaknesses
- Code review on pull requests can slow down the development process due to the extra overhead which isn’t desirable during our short timeframe for the project.
- Everyone here is busy with other school work too, so sometimes you can get blocked waiting on a feature from someone who needs to do other work.
- Inexperience with technologies causes for slow progress and messy code. 
Frequent API changes (in part due to caveats with the technologies in use) requires frequent adaptation from other dependent services.

## Phase 3 plan
For the frontend team, the plan for phase 3 is to finish all of the remaining components that need to be refined or worked on more. Once all the components are fully completed, the final goal for us would be to implement the API calls to the backend to have a fully functional MVP. Right now, only the register page interacts with the API to register users while also providing client-side and backend-side form validation. 

For the backend team, we plan to implement improved permissions and back-end validation logic (for example, edge cases such as disallowing instructors to join groups). We must also add more comprehensive unit testing coverage, which should be easier once the API stabilizes. We will also refactor existing error messages.


