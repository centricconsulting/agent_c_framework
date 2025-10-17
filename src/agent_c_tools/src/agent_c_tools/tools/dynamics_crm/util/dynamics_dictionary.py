ENTITY_MAP = {
    'accounts': {'singular': 'account', 'id_field': 'accountid'},
    'leads': {'singular': 'lead', 'id_field': 'leadid'},
    'opportunities': {'singular': 'opportunity', 'id_field': 'opportunityid'},
    'contacts': {'singular': 'contact', 'id_field': 'contactid'},
    'annotations': {'singular': 'annotation', 'id_field': 'annotationid'},
    'appointments': {'singular': 'appointment', 'id_field': 'activityid'},
    'tasks': {'singular': 'task', 'id_field': 'activityid'},
    'emails': {'singular': 'email', 'id_field': 'activityid'},
    'posts': {'singular': 'post', 'id_field': 'postid'},
    'phonecalls': {'singular': 'phonecall', 'id_field': 'activityid'},
    'cen_serviceofferingcapabilitieses': {'singular': 'cen_serviceofferingcapabilities',
                                          'id_field': 'cen_serviceofferingcapabilitiesid'},
    'businessunits': {'singular': 'businessunit', 'id_field': 'businessunitid'},
    'cen_industryverticalsubs': {'singular': 'cen_industryverticalsub', 'id_field': 'cen_industryverticalsubid'},
    'cen_industryverticals': {'singular': 'cen_industryvertical', 'id_field': 'cen_industryverticalid'},
    'whoami': {'singular': 'whoami', 'id_field': 'UserId'}
}

GUID_STRING_TO_ENTITY_RESOLUTION_MAP = {
    'opportunities': {
        '_owningbusinessunit_value': 'businessunits',
        '_cen_serviceofferingcapabiity1_value': 'cen_serviceofferingcapabilitieses',
        '_cen_serviceofferingcapability2_value': 'cen_serviceofferingcapabilitieses',
        '_cen_serviceofferingcapability3_value': 'cen_serviceofferingcapabilitieses',
        '_cen_centricindustryvertical_value': 'cen_industryverticals',
    },
    'leads': {
        '_owningbusinessunit_value': 'businessunits',
    },
    'accounts': {
        '_owningbusinessunit_value': 'businessunits',
    },
}

LOOKUPS_ID_TO_COMMON_NAME_MAPPING = {
    'businessunits': {'id_field': 'businessunitid', 'name_field': 'name'},
    'cen_serviceofferingcapabilitieses': {'id_field': 'cen_serviceofferingcapabilitiesid',
                                          'name_field': 'cen_name'},
    'cen_industryverticals': {'id_field': 'cen_industryverticalid', 'name_field': 'cen_name'},
}

COMMON_DATE_FIELDS = ['createdon', 'modifiedon', 'scheduledstart', 'scheduledend', 'estimatedclosedate',
                      'actualclosedate']

DEFAULT_FIELDS = {
    'accounts': {
        'select': ['accountid', 'name', 'telephone1', 'emailaddress1', 'websiteurl', 'address1_composite',
                   'revenue', 'numberofemployees', 'industrycode', 'createdon', 'modifiedon', 'statecode',
                   'statuscode', '_owningbusinessunit_value', '_owninguser_value', '_primarycontactid_value',
                   '_cen_centricindustryvertical_value', 'numberofemployees'],
        'expand': {
            'primarycontactid': ['contactid', 'fullname', 'emailaddress1', 'telephone1'],
            'ownerid': ['*'],
            'parentaccountid': ['accountid', 'name'],
        }
    },
    'leads': {
        'select': ['leadid', 'fullname', 'companyname', 'jobtitle', 'emailaddress1', 'telephone1', 'mobilephone',
                   'address1_composite', 'leadsourcecode', 'industrycode', 'revenue', 'numberofemployees',
                   'createdon', 'modifiedon', 'statecode', 'statuscode', 'cen_airelated', 'estimatedamount',
                   'cen_customerpain', 'revenue', '_owningbusinessunit_value'],
        'expand': {
            'ownerid': ['*'],
            'parentaccountid': ['accountid', 'name'],
            'parentcontactid': ['contactid', 'fullname', 'emailaddress1'],
        }
    },
    'opportunities': {
        'select': ['opportunityid', 'name', 'description', 'estimatedvalue', 'estimatedclosedate', 'actualvalue',
                   'actualclosedate', 'stepname', 'opportunityratingcode', 'pricelevelid', 'closeprobability',
                   'createdon', 'modifiedon', 'statecode', 'statuscode', 'proposedsolution', 'cen_projectflag',
                   'cen_opportunitystage', 'cen_weightedrevenue', 'cen_weightedrevenue_base', 'closeprobability',
                   '_cen_serviceofferingcapabiity1_value', '_cen_serviceofferingcapability2_value',
                   '_cen_serviceofferingcapability3_value',
                   'cen_opportunitystagepercentage', '_cen_centricivcapability_value', '_cen_solutionpartner_value',
                   'cen_airelated', 'cen_customerpain', 'customerpainpoints', 'cen_centricswinprobability',
                   '_owningbusinessunit_value',
                   '_cen_centricindustryvertical_value', 'cen_highrisk', 'budgetamount'],
        'expand': {
            'parentaccountid': ['accountid', 'name'],
            'parentcontactid': ['contactid', 'fullname', 'emailaddress1'],
            'ownerid': ['*'],
        }
    },
    'contacts': {
        'select': ['contactid', 'fullname', 'firstname', 'lastname', 'emailaddress1', 'telephone1', 'mobilephone',
                   'jobtitle', 'department', 'address1_composite', 'birthdate', 'anniversary', 'gendercode',
                   'createdon', 'modifiedon', 'statecode', 'statuscode'],
        'expand': {
            'parentcustomerid': ['accountid', 'name'],
            'ownerid': ['systemuserid', 'fullname', 'internalemailaddress']
        }
    },
    'annotations': {
        'select': ['*'],
    },
    'appointments': {
        'select': ['*'],
    },
    'tasks': {
        'select': ['subject', 'description', 'scheduledstart', 'scheduledend', 'createdon',
                   'modifiedon', 'statuscode', 'statecode','_createdby_value'],
        'expand': {
            'createdby': ['fullname']
        }
    },
    'emails': {
        'select': ['*'],
    },
    'posts': {
        'select': ['*'],
    },
    'phonecalls': {
        'select': ['*'],
    },
    'cen_serviceofferingcapabilitieses': {
        'select': ['*'],
    },
    'businessunits': {
        'select': ['*'],
    },
    'cen_industryverticalsubs': {
        'select': ['*'],
    },
    'cen_industryverticals': {
        'select': ['*'],
    },
    'cen_serviceofferings': {
        'select': ['*'],
    },
}
