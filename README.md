# Bytely
URL Shortener Demo Project

Run the API and Client simultaneously. The Client relies on the API.

Thoughts:

The project made me think of Bitly right away, so I looked to them as an example and inspiration for keeping the URL short (hence the name that implies this solution produces larger urls than theirs)

I am using in memory caching, but in the real world we would obviously want to persist this data. In the asbsence of a proper datastore, I am using a simple .GetHashCode() on the URL to generate an 'Id', knowing full well that that isn't a good production solution.
I figured the odds of us coming across the same hash while toying around with it are quite small, and it would suffice for the demo.

Additionally, I would want to add security and the concept of a user and *their* shortened URLS, too. 

I used .net 6 to create a client web application and also an API. The API handles constructing and providing the shortened URLs tot the client application.

If it were taken further, I would build out an identity API that is used across the projects, perhaps using openIdConnect or OAuth, and flesh out the client web app with more features for managing their account and their shortened URLs.
