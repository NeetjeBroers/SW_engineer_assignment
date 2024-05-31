**Equipment Status Application**
-----------------------------

**1: Overview of the Application**
The EquipmentStatusApi Azure Function is a serverless compute service running in the Microsoft Azure Cloud.
By selecting Azure Functions, developers can develop and deploy the solution in a scalable and cost-effective manner.

**1.1: Functionalities**

- Creating of Equipment statuses
- Get specific Equipment status
- Get all Equipment statuses

**2: Setting Up and Running the Application**

**2.1: Environment Setup**
**- Create an Azure function App.**
  - Create New Azure Function App.
   ![image](https://github.com/NeetjeBroers/SW_engineer_assignment/assets/49155482/7cab14fc-de68-4a49-b9b6-16dd01b30fa4)
  - Select Consumption Plan.
    ![image](https://github.com/NeetjeBroers/SW_engineer_assignment/assets/49155482/55910605-e90e-4254-a635-768095830e27)
- Fill in the right configuration and select the 'Review + create' option.
    ![image](https://github.com/NeetjeBroers/SW_engineer_assignment/assets/49155482/d9525ed1-66ed-4519-b9c3-ce4539eb3c88)
- Under the Deployment tab you select the Basic Authentication on.
![image](https://github.com/NeetjeBroers/SW_engineer_assignment/assets/49155482/5f60da8c-293b-47bf-95e5-175b730827ef)
- Do a final control of the settings and create the Azure Function App.
 ![image](https://github.com/NeetjeBroers/SW_engineer_assignment/assets/49155482/dee24a85-6c25-4998-a78f-9828e938f42f)

- Navigate to the created Azure Function App and add application setting.
  ![image](https://github.com/NeetjeBroers/SW_engineer_assignment/assets/49155482/277c1ba9-2972-43da-85c5-0ac55b1df90d)


**2.2: Create an Azure storage for the Azure Storage Table Database.**
- Create a storage account.        
 ![image](https://github.com/NeetjeBroers/SW_engineer_assignment/assets/49155482/1bb0aa4c-9098-4c46-966c-0a39147ccfe3)
- Configure the storage account details and create.
 ![image](https://github.com/NeetjeBroers/SW_engineer_assignment/assets/49155482/ab21cd52-b487-4082-946b-84f3c16715ce)
- navigate to the created storage account
![image](https://github.com/NeetjeBroers/SW_engineer_assignment/assets/49155482/8a55c3f5-5a7b-4287-8078-d93594aa8439)
- go to the Acces Keys and copy the Key1 connection string.
 ![image](https://github.com/NeetjeBroers/SW_engineer_assignment/assets/49155482/275ecd09-eb7e-49ab-b539-fe83a1a91d0c)

**2.3: Deploy the Function App to the App in Azure**
- Select the Azure Function App in Visual Studio and right click to get the publish option.
![image](https://github.com/NeetjeBroers/SW_engineer_assignment/assets/49155482/939a7af8-9da0-406e-9cdb-87908b8a7d65)
- Select the Azure option.
![image](https://github.com/NeetjeBroers/SW_engineer_assignment/assets/49155482/5f8ce406-5a45-4e5d-bae0-46d8542cd355)
- Select the created Azure Function Type (In this case Linux).
![image](https://github.com/NeetjeBroers/SW_engineer_assignment/assets/49155482/2a2ec050-6598-427f-b17f-4fe8b14d488e)
- Select the Created Azure Function App in the Resource group.
![image](https://github.com/NeetjeBroers/SW_engineer_assignment/assets/49155482/a94a9eba-8e1b-4791-8a6e-3fe4c73d7d32)
- Disable API management.
![image](https://github.com/NeetjeBroers/SW_engineer_assignment/assets/49155482/120fa458-e6da-44af-93e0-f2d0f49c7b7b)
- Finally finish the publishing profile.
![image](https://github.com/NeetjeBroers/SW_engineer_assignment/assets/49155482/be6ccea2-7157-4768-adcd-059001f38b53)
- Finally Publish the code to the Function App with the Publish button
![image](https://github.com/NeetjeBroers/SW_engineer_assignment/assets/49155482/d2c05ded-e875-467b-8bf6-a105d1f31610)

**2.4: Running the application out of Visual Studio**
- Run the application with the correct settings in the local.settings.json
  ![image](https://github.com/NeetjeBroers/SW_engineer_assignment/assets/49155482/02e5653c-9063-4cfe-8d34-1f3abe4ac48b)
- After running the Application you have 2 options
  ![image](https://github.com/NeetjeBroers/SW_engineer_assignment/assets/49155482/742edb5a-b88f-45df-920c-cf8657b6dcd6)
    1. Using the SwaggerUI to test the functionality
       ![image](https://github.com/NeetjeBroers/SW_engineer_assignment/assets/49155482/daa66c7a-2904-43e3-a423-bcc0267e2dc0)
    2. Using Postman to send in the data
       ![image](https://github.com/NeetjeBroers/SW_engineer_assignment/assets/49155482/0aac1b5e-2164-4ff1-8555-b7c40b3a608e)


**3: Brief discussion for subjects > Database Choice and Architectural pattern**

**3.1: Database Choice**
- For this project I have selected Azure Table Storage as the Database, this is an non-relational Database.
  This type of Database is quick for saving the statuses, in this project I have not included the Tables (Equipment,Status) to keep the simplicity of the project.
  - 3.1.1 Database Scheme:
    - Below Database scheme shows the fields in the Azure Table EquipmentStatus                 
      ![image](https://github.com/NeetjeBroers/SW_engineer_assignment/assets/49155482/83bd559c-9344-4aad-a4b2-d601189df4d3)
      
**3.2: Architectural pattern**
- Within developing the Azure function I have gone with the Architectural pattern of writing a combination of Serverless functions and Microservices.
 The main goal with developing is writing short functions that have a self explanatory name and written functionality.
