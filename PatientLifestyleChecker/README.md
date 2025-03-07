# AL_PatientLifeStyle

## Introduction

This project is a part of the course "Advanced Learning" at the University of Applied Sciences Technikum Wien. The goal of this project is to develop a machine learning model that predicts the lifestyle of a patient based on the given data. The data is provided by the company "Semeion Research Center" and contains information about the patient's


##Observations

In part two of the task the Question scoring age bands overlap.  There is a band 41-65 and a 64+, so someone who is 65 could be in either band.  This could be a problem.  Also the band 64+ could mean greater than 64 or 64 and greater.  I have taken the decision to assume the bands should be 41-64 and 65 and greater and also that for a given patient they can only score in one age band.

One of the response messages has a typo "eligble" should be "eligible";

One of the outcome messages reads, "improve you quality of life" which should be "your".

There is nothing in the spec that suggests that persisent storage is a requirement or desirable and I have taken the assumption that it is actively not wanted for whatever reason, GDPR or just wanting to be very light tu=ouch to encourage patient responses.  Other similar projects may require not only succesful responses to be logged to a database but perhaps also useage and failed path data to be stored.


I have made the assumption that as this survey is not secured behind any sort of login that it may be used in a kiosk or shared web browser situation so have deliberately chosen to revert to the initial landing screen after suitable timeouts (5 seconds for the outcome page and 30 for the survey page ), these are hardcoded for now but could configured as startup config variables going forward.

##Features

The API key will need to be added to a appsettings.Development.json file whick can be copied from the appsettings.Example.json provided.

I have provided a solution that takes all it's Survey knowledge from a single json file, allowing the survey to be changed without rebuilding or deploying code.  The questions, the age banding for scoring and the scores themselves are all manageable.  The outcome thresholds and messages are also managed in the same file.  It also means additional yes/no questions can be added without changing the code.  At present the question type is limited to yes no answers but this could be developed futher if required.  I have provided a second sample json survey file "survey2.json", if you change the existing "survey.json" to be "survey.json.old" and the second file to be "survey.json" whilst the app is running the next user will get the alternate question set.  This is likely to cause issues with users currently in the system as the scoring reloads the json file at present.  This could be easily fixed so a user journey tracks a json file version to complete their journey whilst still allowing new journeys to use a new file.  Edge case issues but there options for providing a clean swap over, the altenative is make the switch when no one is on the site! This feature allows for a change to age groups or scores for individual questions or adding extra questions altogether.

##Future Enhancements

On the Survey page the radio buttons default to selecting the false/no option.  I have a big hammer approach to solve this involving a hidden field to store the question answer value and updating this via jquery when the radio buttons are checked/unchecked, but this hasn't been implemented.

I haven't given any consideration to either accessibility standards or specific mobile or non browser use.

The Bootstrap Datepicker is a bit clunky and could be replaced with a more modern date picker.  At present it defaults to todays date and does allow future dates.  On form refresh it would be better if it showed the previously selected date.

At present whilst extra questions can be added, these will all be presented on a single page that could quickly become quite unwieldy, it would be great to add in some form of pagination when the page becomes too great.  I thought about using a single question per page model which avoids this but for simplicity chose to add all questions to the same showing of the page. 

