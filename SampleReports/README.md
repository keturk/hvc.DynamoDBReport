# Prerequisites
Sample reports provided here use [AWS sample data models for NoSQL Workbench](https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/workbench.SampleModels.html). 

You can use [NoSQL Workbench for DynamoDB](https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/workbench.html) to install these tables to your AWS account.

# Sample Reports

[Employee Data Model](https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/workbench.SampleModels.html#workbench.SampleModels.EmployeeDataModel)
    
* [Employee.dynaq](./Employee.dynaq)
    * Modular versions
        * [employees.py](./Modular/employees.py) 
        * [employee_direct_reports.py](./Modular/employee_direct_reports.py) 
    * Monolithic versions
        * [employees.py](./Monolithic/employees.py) 
        * [employee_direct_reports.py](./Monolithic/employee_direct_reports.py) 

[Credit card offers data model](https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/workbench.SampleModels.html#workbench.SampleModels.CreditCardOffersDataModel)

* [CreditCardOffers.dynaq](./CreditCardOffers.dynaq) 
    * Modular version
        * [credit_card_offers.py](./Modular/credit_card_offers.py) 
    * Monolithic version
        * [credit_card_offers.py](./Mononlithic/credit_card_offers.py)

[Bookmarks data model](https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/workbench.SampleModels.html#workbench.SampleModels.BookmarksDataModel)

* [CustomerBookmark.dynaq](./CustomerBookmark.dynaq) 
    * Modular versions
        * [customer_bookmarks.py](./Modular/customer_bookmarks.py) 
        * [customer_bookmarks_by_customer_folder.py](./Modular/customer_bookmarks_by_customer_folder.py) 
        * [customer_bookmarks_by_email.py](./Modular/customer_bookmarks_by_email.py)
        * [customer_bookmarks_by_url.py](./Modular/customer_bookmarks_by_url.py)
    * Monolithic versions
        * [customer_bookmarks.py](./Monolithic/customer_bookmarks.py)
        * [customer_bookmarks_by_customer_folder.py](./Monolithic/customer_bookmarks_by_customer_folder.py) 
        * [customer_bookmarks_by_email.py](./Monolithic/customer_bookmarks_by_email.py)
        * [customer_bookmarks_by_url.py](./Monolithic/customer_bookmarks_by_url.py)

[Discussion forum data model](https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/workbench.SampleModels.html#workbench.SampleModels.DiscussionForumDataModel)
* [Forum.dynaq](./Forum.dynaq)
    * Modular version
        * [forum.py](./Modular/forum.py)
    * Monolithic version
        * [forum.py](./Monolithic/forum.py)

* [Reply.dynaq](./Reply.dynaq)
    * Modular versions
        * [replies.py](./Modular/replies.py)
        * [replies_posted_by.py](./Modular/replies_posted_by.py)
    * Monolithic versions
        * [replies.py](./Monolithic/replies.py)
        * [replies_posted_by.py](./Monolithic/replies_posted_by.py)

[Ski resort data model](https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/workbench.SampleModels.html#workbench.SampleModels.SkiResortDataModel)
* [SkiLift.dynaq](./SkiLift.dynaq)
    * Modular version
        * [ski_lift_by_riders.py](./Modular/ski_lift_by_riders.py)
    * Monolithic version
        * [ski_lift_by_riders.py](./Monolithic/ski_lift_by_riders.py)

[Music library data model](https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/workbench.SampleModels.html#workbench.SampleModels.MusicLibraryDataModel)
* [Songs.dynaq](./Songs.dynaq)
    * Modular versions
        * [songs.py](./Modular/songs.py)
        * [songs_by_month.py](./Modular/songs_by_month.py)
        * [songs_by_month_and_downloads.py](./Modular/songs_by_month_and_downloads.py)
    * Monolithic versions
        * [songs.py](./Monolithic/songs.py)
        * [songs_by_month.py](./Monolithic/songs_by_month.py)
        * [songs_by_month_and_downloads.py](./Monolithic/songs_by_month_and_downloads.py)
