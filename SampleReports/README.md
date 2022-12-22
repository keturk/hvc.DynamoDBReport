# Sample Reports

## Prerequisites
Sample reports provided here use [AWS sample data models for NoSQL Workbench](https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/workbench.SampleModels.html). 

You can deploy below data models to your AWS account with [NoSQL Workbench for DynamoDB](https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/workbench.html).

## Standard command line arguments of generated Python code
In addition to custom command line arguments, generated code defines three optional command line arguments.
* --aws <aws_profile> : is an optional parameter which can be used to specify aws_profile that will be used. If not defined, script assume 'default' as value.
* --csv <csv_filename>: if defined, script will create a csv file with given name
* --silent: if defined, script will not create PrettyTable output to the console.

## How to define custom command line arguments
DynaQ DSL makes your generated Python scripts versatile with custom command line arguments. Custom command line arguments can be defined in two different ways.

### Inside a quoted string
DynaQ parser looks for identifiers inside with curly brackets inside qouted strings.

Ability to define custom command line argument inside a quoted string makes table naming more flexible. If your environment requires a naming convention for tables, you can define custom command line arguments to meet that requirement.

Below report is defining three different custom command line arguments for the generated script.

```

Report SampleReport {
    Select id, name, address From 'hvc-{env}-table-{region}'
    With id == '{id}'
}

```

```

python .\sample_report.py --id 1 --env dev --region us-east-1

```

### Standalone identifier as value part of a criterion
Another way to define custom command line argument is to use an identifier as value field of a criterion. This gives you the ability define integer input values for your queries.

In below report, DynaQ DSL treats `lift` argument as string, `total` argument as integer.

```

Report SkiLiftByRiders {
    Select Lift, Metadata, TotalUniqueLiftRiders From SkiLifts
    With SkiLiftsByRiders(Lift == '{lift}' And TotalUniqueLiftRiders >= total)
}

```

```

python .\ski_lift_by_riders.py --lift 'Lift 23' --total 500

```



## Sample Data Models and Reports
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
