# Travel Diary


## Table of Contents
* [General Info](#general-information)
* [Technologies Used](#technologies-used)
* [Features](#features)
* [Screenshots](#screenshots)
* [Setup](#setup)
* [Usage](#usage)
* [Project Status](#project-status)
* [Room for Improvement](#room-for-improvement)
* [Acknowledgements](#acknowledgements)
* [Contact](#contact)


## General Information
Travel Diary is a Rest API project witch can be use as Api for social media dedicated to users who share stories from travels as Diaries. 



## Technologies Used
- Azure.Storage - version 12.15.0
- MSSql
- EntityFrameworkCore - version 7.0.10
- EntityFrameworkCore.InMemory - version 7.0.10
- Xunit - version 2.5.0
- Moq - version 4.20.69
- MediatR - version 12.1.1
- FluentValidation - version 11.7.1
- Mapster - Version 7.3.0

- FluentAssertions


## Features
List the ready features here:
### Account Menagment
- Register User Account route  - Post /Api/Account/Register
- Login User Account route - Post /Api/Account/Login 
- Update User Details route - Put /Api/Account
- Delete User Account route - Delete /Api/Account
### Diary Menagment
- Create Diary route - Post /Api/Diary
- Update Diary description route - Put /Api/Diary/{diaryId}/Description
- Delete Diary route route - Delete /Api/Diary/{diaryId}
- Get Diary by Id route - Get /Api/Diary/{id}
- Get Diaries route - Get /Api/Diary
### Entry Menagment
- Add Entry to diary route - Post /Api/Diary/{diaryId}/Entry
- Update Entry route - Put /Api/Entry/{entryId}
- Delete Entry route - Delete /Api/Entry/{entryId}
### Photo Menagment
- Add Photo to entry route - Post /Api/Entry/{entryId}/Photo
- Update Photo details route - Put /Api/Photo/Id
- Download Photo route -Get /Api/Photo/{photoId}
- Get photo details -route -Get /Api/Photo/{photoId}/Details

