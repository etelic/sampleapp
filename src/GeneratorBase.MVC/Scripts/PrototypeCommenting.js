function PrototypeCommenting(prototypeurl, isClose,e)
{
    $(document).contextmenu(function () {
        return isClose;
    });
  
        if (e.which == 3)
        {
            var elementId = (e.target || e.srcElement).id;
            var elementText = ($(e.target || e.srcElement).text());
            var elementValue = ($(e.target || e.srcElement).val());
            var elementType = (e.target || e.srcElement).nodeName;
            var btnType = ($(e.target || e.srcElement).attr("type"));
            if (elementType == undefined)
                return isClose;
            if (elementType.toString() != "TEXTAREA" && elementType.toString() != "LABEL" && elementType.toString() != "A" && elementType.toString() != "BUTTON" && elementType.toString() != "INPUT") {
                elementText = "";
                elementType = "Page";
            }
            // return false;
            if (elementText != undefined && elementText.length > 0) {
                OpenNotes(prototypeurl, elementText.trim(), elementType.trim(), e);
            }
            else
                if (elementId != undefined && elementId.length > 0)
                    OpenNotes(prototypeurl, elementId.trim(), elementType.trim(), e);
                else
                    if (elementValue != undefined && elementValue.length > 0) {
                        if (btnType != null) {
                            elementType = btnType;
                        }
                        OpenNotes(prototypeurl, elementValue.trim(), elementType.trim(), e);
                    }
                    else {
                        OpenNotes(prototypeurl, "", "", e);
                    }
        }
        else if (e.which == 1)
        {
            var elementId = (e.target || e.srcElement).id;
            var elementText = ($(e.target || e.srcElement).text());
            var elementValue = ($(e.target || e.srcElement).val());
            var elementType = (e.target || e.srcElement).nodeName;
            var btnType = ($(e.target || e.srcElement).attr("type"));
            
            if (elementType == undefined) 
            {
                return isClose;
            }
            
            if (elementType.toString() == "TEXTAREA" || elementType.toString() == "SELECT" || elementType.toString() == "RADIO" || elementType.toString() == "CHECKBOX" || elementType.toString() == "LABEL" || elementType.toString() == "A" || elementType.toString() == "BUTTON" || elementType.toString() == "INPUT" || elementType.toString() == "OPTION") {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
            return isClose;
    
}



