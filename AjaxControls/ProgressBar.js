// Register the namespace for the control.
Type.registerNamespace("Gafware.Modules.DMS.AjaxControls");

Gafware.Modules.DMS.AjaxControls.ProgressBar = function (element) {
    Gafware.Modules.DMS.AjaxControls.ProgressBar.initializeBase(this, [element]);
    this._pollInterval = 0;
    this._processName = null;
    this._finishLink = '';
    this._portalId = 0;
    this._tabModuleId = 0;
    this._portalWideRepository = false;
    this._controlPath = '';
    this._filePath = '';
    this._subFolderIsDocumentName = false;
    this._subFolderIsTag = false;
    this._prependSubFolderName = false;
    this._seperator = '';
    this._firstLevel = 0;
    this._activationDate = null;
    this._expirationDate = null;
    this._ownerId = 0;
    this._searchable = true;
    this._useCategorySecurityRoles = true;
    this._securityRoleId = 0;
    this._categories = [];
    this._filesImported = 0;
}

Gafware.Modules.DMS.AjaxControls.ProgressBar.prototype = {
    "initialize" : function() {
        Gafware.Modules.DMS.AjaxControls.ProgressBar.callBaseMethod(this, "initialize");
	    this._element.ProgressBar = this;
	    // I'm going to add a function right to the element itself called
	    // "Start". This way you can just call $get('blah').Start();
	    this._element.BulkImport = function () {
		    var element = this;
            Gafware.Modules.DMS.DMSController.ImportFiles(element.ProgressBar._controlPath, element.ProgressBar._filePath, element.ProgressBar._subFolderIsDocumentName,
                element.ProgressBar._subFolderIsTag, element.ProgressBar._prependSubFolderName, element.ProgressBar._seperator, element.ProgressBar._firstLevel,
                element.ProgressBar._activationDate, element.ProgressBar._expirationDate, element.ProgressBar._ownerId, element.ProgressBar._searchable,
                element.ProgressBar._useCategorySecurityRoles, element.ProgressBar._securityRoleId, element.ProgressBar.categories, element.ProgressBar._portalId,
                element.ProgressBar._tabModuleId, element.ProgressBar._portalWideRepository, function (result) {
                element.ProgressBar._processName = result;
		        doPolling(element);
		    });
	    }
        var doPolling = function (element) {
            Gafware.Modules.DMS.DMSController.GetFilesImportedCount(element.ProgressBar._processName, function (result) {
                element.ProgressBar._filesImported = result;
            });
            Gafware.Modules.DMS.DMSController.GetImportFilesProgress(element.ProgressBar._processName, function (result) {
                element.innerHTML = result + "%";
                if (result < 100) {
                    setTimeout(function () { doPolling(element); }, element.ProgressBar._pollInterval);
                } else {
                    eval(element.ProgressBar._finishLink);
                }
            });
        }
    },

    // These "get" and "set" accessors are required because I added
    // a property in the code behind file for the control with this
    // line of code: descriptor.AddProperty("pollInterval", this.PollInterval);

    "get_pollInterval" : function() {
	    return this._pollInterval;
    },

    "set_pollInterval" : function(value) {
	    if (this._pollInterval !== value) {
		    this._pollInterval = value;
		    this.raisePropertyChanged("pollInterval");
	    }
    },

    "get_processName" : function() {
	    return this._processName;
    },

    "set_processName" : function(value) {
	    if (this._processName !== value) {
		    this._processName = value;
		    this.raisePropertyChanged("processName");
	    }
    },

    "get_subject": function () {
        return this._subject;
    },

    "set_subject": function (value) {
        if (this._subject !== value) {
            this._subject = value;
            this.raisePropertyChanged("Subject");
	    }
    },

    "get_body": function () {
        return this._body;
    },

    "set_body": function (value) {
        if (this._body !== value) {
            this._body = value;
            this.raisePropertyChanged("Body");
        }
    },

    "get_startDate": function () {
        return this._startDate;
    },

    "set_startDate": function (value) {
        if (this._startDate !== value) {
            this._startDate = value;
            this.raisePropertyChanged("StartDate");
        }
    },

    "get_endDate": function () {
        return this._endDate;
    },

    "set_endDate": function (value) {
        if (this._endDate !== value) {
            this._endDate = value;
            this.raisePropertyChanged("EndDate");
        }
    },

    "get_jobIds": function () {
        return this._jobIds;
    },

    "set_jobIds": function (value) {
        if (this._jobIds !== value) {
            this._jobIds = value;
            this.raisePropertyChanged("JobIds");
        }
    },

    "get_statusId": function () {
        return this._statusId;
    },

    "set_statusId": function (value) {
        if (this._statusId !== value) {
            this._statusId = value;
            this.raisePropertyChanged("StatusId");
        }
    },

    "get_finishLink": function () {
        return this._finishLink;
    },

    "set_finishLink": function (value) {
        if (this._finishLink !== value) {
            this._finishLink = value;
            this.raisePropertyChanged("FinishLink");
        }
    }
}

// Register the class as a type that inherits from Sys.UI.Control.
Gafware.Modules.DMS.AjaxControls.ProgressBar.registerClass("Gafware.Modules.DMS.AjaxControls.ProgressBar", Sys.UI.Control);

if (typeof(Sys) !== "undefined") Sys.Application.notifyScriptLoaded();