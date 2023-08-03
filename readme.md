# Welcome to Howler!

Howler is a full-stack web application built with .NET/C# Web Api in the back end, and React for a front end. The concept behind Howler is that it is essentially a forum themed for werewolves and werewolf enthusiasts.

## Packs
Every werewolf needs a good pack (unless they're more of a... lone wolf 🐺). Pack in Howler are essentially private groups, which each have a member-only board pseudo-administered by the Pack's Leader. A user can join a pack, or leave one, and if they're not already in a pack, they can decide to make their own. Packs can only be edited or deleted by the Pack Leader.

## Boards, Posts, Comments
Boards are, simply put, a collection of posts, in which there is a board owner that acts as an administrator for that board. Users can make posts on any public board, or even make new public boards, and also make posts on those boards. When the user views a post, they also can write comments. Users can edit their own posts, comments, and boards to their liking (as well as delete them, if they belong to the user). Board Owners can delete any post on their board, as well as any comments on any post on their board (they can only edit the comments or posts if they belong to the owner itself, though). 


# Getting started

First, you'll want to open your favorite terminal and navigate to where you would like to clone the files. Do a ``git clone`` , and head into your new repository.

From there, you'll need to do a few things: 

 1. If you have Visual Studio, once you're in the directory in your terminal, you can run ``start Howler.sln`` If you don't have Visual Studio, you should install it!
 2. Open the ``SQL`` folder, and run the database creation script. Save the seed data script for **later**. If you don't have SQL Server on your machine, you'll need to install it and connect the script to your SQL Server.
 3. Make a project on Google Firebase, enable authentication, and create a new user. Make sure the email and password are something you can remember!
 4. You'll need to add a ``.env`` file to the client folder, and it's contents should resemble ``REACT_APP_API_KEY=[YOUR_FIREBASE_WEB_API_KEY_HERE]``, where you replace ``[YOUR_FIREBASE_WEB_API_KEY_HERE]``(including the []) with your Firebase Project's Web API Key.
 5. In Visual Studio, edit the solution's ``appsettings.json`` file and edit the ``FirebaseProjectId`` to have your Firebase project's id in ``"quotes"``
 6. In the ``SQL`` folder, open the seed data script, and change the information on the *first line* to match the user you just created in Google Firebase - specifically, you need to set the email to be the same email as the user you made in Firebase, and you need to copy/paste the user's Firebase Id into the appropriate section. You can also change the user's display name and profile picture while you're there, if you like.
 7. Now you run the seed data script, after you have edited the user's details.
 8. ``cd`` into the ``/client`` folder, and run ``npm i`` to download dependencies for the client portion of the project.
 9. Back in Visual Studio, you should spin up the server, you can do it by pressing the play button at the top. ***Note: Make sure you are running Howler, NOT IIS Express - change it in the little dropdown if you need to. *** You should see a Swagger page pop up in your browser, to confirm it is on, and to view the back-end's endpoints.
 10. Back in the terminal, if you're still in the ``client`` folder, you should be able to type ``npm start``, which should open up the client in your browser.
 11. Log into the client on your browser and explore! Awoooooo! 🐺🌕