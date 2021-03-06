# Documentation

## Requirements
- NodeJS (https://nodejs.org/en/download/)
- .NET (https://www.microsoft.com/net/download/)
- MongoDB (https://www.mongodb.com/download-center#community)

## Setup
# Front End
```
git clone https://github.com/csc302-winter-2018/proj-Da_Krew.git
cd proj-Da_Krew/da-krew-app/ClientApp
npm install
```
# Back End
Back end requires microsoft net framework and `dotnet` on PATH

## Running
- Have an instance of MongoDB running in the background, you could do this by opening your terminal and running `mongod` once you've installed MongoDB
- Start the backend by navigating to a given microservice directory (you can perform this in a separate terminal window for every microservice) (such as `microservices\UserService` and start the microservice by using the following command: `dotnet run`
- Start the frontend by going into your cloned repository and going into `da-krew-app/ClientApp`. Once you're in the directory, simply run `npm start`

## Frontend Layout
```
/node_modules
    Created and populated with required modules when executing `npm install`
/public
    favicon.ico
    index.html <-- Meta tags for SEO, entry point for React
    manifest.json
/src
    /authentication <-- Contains components such as login and register
    /components <-- Components used through the application such as the homepage, dashboard, etc
    /credentialStore <-- Used to store credential information in Redux
    App.jsx <-- Contains the routing
    index.js <-- Where react renders itself
package-lock.json
package.json <-- Packages used in our project
README.md

```

## Additional Frontend Documentation
Additional frontend documentation that's more extensive can be found [here](https://github.com/csc302-winter-2018/proj-Da_Krew/blob/master/da-krew-app/ClientApp/README.md)

## Backend 
We recomment setting up the microservices behind a load balancer such as NGINX which would transparently redirect traffic to the correct microservice by pattern matching on the route (i.e. redirect /api/User* to User microservice.
We will add an easier automated method for this in the future versions.
