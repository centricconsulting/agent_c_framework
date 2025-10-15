//function expandCollapse(prefix, expandOrCollapse) {
//    var panelId;
//    var expandedOrCollapsedFlagId;
//    if (prefix == 'ph') {
//        panelId = pnlPolicyholders;
//        expandedOrCollapsedFlagId = hdnPhExpandedOrCollapsedId;
//    } else if (prefix == 'd') {
//        panelId = pnlDrivers;
//        expandedOrCollapsedFlagId = hdnDExpandedOrCollapsedId;
//    } else if (prefix == 'v') {
//        panelId = pnlVehicles;
//        expandedOrCollapsedFlagId = hdnVExpandedOrCollapsedId;
//    } else if (prefix == 'l') {
//        panelId = pnlLocations;
//        expandedOrCollapsedFlagId = hdnLExpandedOrCollapsedId;
//    } else if (prefix == 'c') {
//        //added else if 1/27/2014
//        panelId = pnlCoverages;
//        expandedOrCollapsedFlagId = hdnCExpandedOrCollapsedId;
//    }
//    if (panelId) {
//        var expImgId = prefix + '_expand';
//        var conImgId = prefix + '_collapse';
//        var expImg = document.getElementById(expImgId);
//        var conImg = document.getElementById(conImgId);

//        if (expImg && conImg) {
//            if (typeof (expandOrCollapse) == 'undefined') {
//                expandOrCollapse = 'expand';
//            }
//            if (expandOrCollapse == 'collapse') {
//                //collapse
//                ShowOrHideBlockElement(panelId, false)
//                ShowOrHideInlineElement(expImgId, true)
//                ShowOrHideInlineElement(conImgId, false)
//                if (document.getElementById(expandedOrCollapsedFlagId)) {
//                    document.getElementById(expandedOrCollapsedFlagId).value = 'collapsed';
//                }
//            } else {
//                //expand
//                ShowOrHideBlockElement(panelId, true)
//                ShowOrHideInlineElement(expImgId, false)
//                ShowOrHideInlineElement(conImgId, true)
//                if (document.getElementById(expandedOrCollapsedFlagId)) {
//                    document.getElementById(expandedOrCollapsedFlagId).value = 'expanded';
//                }
//            }
//        }
//    }
//}
function retainExpandedOrCollapsedSections() {
    //var hdnPhExpandedOrCollapsed = document.getElementById(hdnPhExpandedOrCollapsedId);
    //var hdnDExpandedOrCollapsed = document.getElementById(hdnDExpandedOrCollapsedId);
    //var hdnVExpandedOrCollapsed = document.getElementById(hdnVExpandedOrCollapsedId);
    //var hdnLExpandedOrCollapsed = document.getElementById(hdnLExpandedOrCollapsedId);
    //var hdnCExpandedOrCollapsed = document.getElementById(hdnCExpandedOrCollapsedId); //added 1/27/2014

    //if (hdnPhExpandedOrCollapsed) {
    //    if (hdnPhExpandedOrCollapsed.value == 'expanded') {
    //        expandCollapse('ph');
    //    } else if (hdnPhExpandedOrCollapsed.value == 'collapsed') {
    //        expandCollapse('ph', 'collapse');
    //    }
    //}
    //if (hdnDExpandedOrCollapsed) {
    //    if (hdnDExpandedOrCollapsed.value == 'expanded') {
    //        expandCollapse('d');
    //    } else if (hdnDExpandedOrCollapsed.value == 'collapsed') {
    //        expandCollapse('d', 'collapse');
    //    }
    //}
    //if (hdnVExpandedOrCollapsed) {
    //    if (hdnVExpandedOrCollapsed.value == 'expanded') {
    //        expandCollapse('v');
    //    } else if (hdnVExpandedOrCollapsed.value == 'collapsed') {
    //        expandCollapse('v', 'collapse');
    //    }
    //}
    //if (hdnLExpandedOrCollapsed) {
    //    if (hdnLExpandedOrCollapsed.value == 'expanded') {
    //        expandCollapse('l');
    //    } else if (hdnLExpandedOrCollapsed.value == 'collapsed') {
    //        expandCollapse('l', 'collapse');
    //    }
    //}

    ////added 1/27/2014
    //if (hdnCExpandedOrCollapsed) {
    //    if (hdnCExpandedOrCollapsed.value == 'expanded') {
    //        expandCollapse('c');
    //    } else if (hdnCExpandedOrCollapsed.value == 'collapsed') {
    //        expandCollapse('c', 'collapse');
    //    }
    //}

    //added 2/21/2014
    RetainExpandedCollapsedSublistsForElements('li');
    RetainExpandedCollapsedSublistsForElements('div'); //added 3/14/2014 to handle for main sections (since expand/collapse image area is inside div instead of right under li)
}
function CheckForExpandOrCollapse() {
    var expandOrCollapseAllFlag = document.getElementById(hdnExpandOrCollapseAllFlagId);
    if (expandOrCollapseAllFlag) {
        if (expandOrCollapseAllFlag.value == 'expand') {
            ExpandAll();
            expandOrCollapseAllFlag.value = '';
            //document.getElementById(hdnExpandOrCollapseAllFlagId).value = '';
            return;
        } else if (expandOrCollapseAllFlag.value == 'collapse') {
            CollapseAll();
            expandOrCollapseAllFlag.value = '';
            //document.getElementById(hdnExpandOrCollapseAllFlagId).value = '';
            return;
        }
        //retainExpandedOrCollapsedSections(); //1/22/2014 note: would probably be better after next } since it's not dependent on IF
    }
    retainExpandedOrCollapsedSections();
}
function ExpandAll() {
    //expandCollapse('ph');
    //expandCollapse('d');
    //expandCollapse('v');
    //expandCollapse('l');
    //expandCollapse('c'); //added 1/27/2014
    expandCollapseAllSublistsForElements('li'); //added 2/21/2014
    expandCollapseAllSublistsForElements('div'); //added 3/14/2014 to handle for main sections (since expand/collapse image area is inside div instead of right under li)
}
function CollapseAll() {
    //expandCollapse('ph', 'collapse');
    //expandCollapse('d', 'collapse');
    //expandCollapse('v', 'collapse');
    //expandCollapse('l', 'collapse');
    //expandCollapse('c', 'collapse'); //added 1/27/2014
    expandCollapseAllSublistsForElements('li', 'collapse'); //added 2/21/2014
    expandCollapseAllSublistsForElements('div', 'collapse'); //added 3/14/2014 to handle for main sections (since expand/collapse image area is inside div instead of right under li)
}

//added 2/20/2014
function expandCollapseSubLists(el, expandOrCollapse) {
    //updated 5/15/2014
    //if (IsParentListItemDisabled(el) == true) {
    //    return;
    //}
    if (HasAnyDisabledParentListItem(el) == true) {
        return;
    }

    if (el) {
        if (typeof (expandOrCollapse) == 'undefined') {
            expandOrCollapse = 'expand';
        }
        var showOrHide;
        if (expandOrCollapse == 'collapse') {
            //collapse
            showOrHide = 'hide';
        } else {
            //expand
            showOrHide = 'show';
        }
        showHideRelatedChildPanels(el, showOrHide);
    }
}
function showHideRelatedChildPanels(child, showOrHide, panelsOnly) {
    if (child) {
        if (typeof (showOrHide) == 'undefined') {
            showOrHide = 'show';
        }
        if (typeof (panelsOnly) == 'undefined') {
            panelsOnly = false;
        }
        var parent = GetParentElementForChild(child);
        if (parent) {
            switch (parent.nodeType) {
                case 3:
                    //text Node
                    break;
                case 1:
                    //Element node
                    if (parent.id.length > 0) {
                        if (parent.id.indexOf('SubLists_expandCollapse') != -1) {
                            //updated 3/14/2014 for main sections (expand/collapse image area is inside div, which is at the same level as the panel); previous logic is in ELSE
                            //alert('showHideRelatedChildPanels... parent id: ' + parent.id);
                            if (parent.id.indexOf('Main') != -1 && parent.id.indexOf('Section') != -1) {
                                var parent2 = GetParentElementForChild(parent);
                                if (parent2) {
                                    //alert('showHideRelatedChildPanels... has main/section... using parent2: ' + parent2.id);
                                    showHideRelatedChildPanels(parent2, showOrHide, true);
                                } else {
                                    //alert('showHideRelatedChildPanels... has main/section but not parent2... using normal parent');
                                    showHideRelatedChildPanels(parent, showOrHide, true);
                                }
                            } else {
                                //need to run again since panels are at same level as expand/collapse image area
                                //alert('showHideRelatedChildPanels... no main/section... using normal parent');
                                showHideRelatedChildPanels(parent, showOrHide, true);
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
            var children = parent.childNodes;
            if (children) {
                for (i = 0; i < children.length; i++) {
                    var currChild = children[i];
                    if (currChild) {
                        switch (currChild.nodeType) {
                            case 3:
                                //text Node
                                break;
                            case 1:
                                //Element node
                                if (currChild.id.length > 0) {
                                    if (currChild.id.indexOf('pnl') != -1) {//panel
                                        if (showOrHide == 'hide') {
                                            //hide (collapse)
                                            //ShowOrHideBlockElement(currChild.id, false);
                                            //updated 3/13/2014 to bypass ShowOrHide function
                                            currChild.style.display = "none";
                                        } else {
                                            //show (expand)
                                            //ShowOrHideBlockElement(currChild.id, true);
                                            //updated 3/13/2014 to bypass ShowOrHide function
                                            currChild.style.display = "block";
                                        }
                                    } else if (currChild.id.indexOf('SubLists_expandCollapse') != -1) {//expand/collapse image area
                                        //ignore this... just needed so it doesn't jump in SubLists_expand ELSEIF for this element
                                    } else if (currChild.id.indexOf('SubLists_expand') != -1) {//expand  image
                                        if (panelsOnly == false) {
                                            if (showOrHide == 'hide') {
                                                //hide (collapse)
                                                //ShowOrHideInlineElement(currChild.id, true);
                                                //these images don't runat server so they all have the same id; function just updates 1st one always
                                                currChild.style.display = "inline";
                                            } else {
                                                //show (expand)
                                                //ShowOrHideInlineElement(currChild.id, false);
                                                //these images don't runat server so they all have the same id; function just updates 1st one always
                                                currChild.style.display = "none";
                                                //updated 3/12/2014 so it still retains space... specific to expand/collapse images and X's or remove/delete buttons
                                                //currChild.style.display = "hidden";
                                            }
                                        }
                                    } else if (currChild.id.indexOf('SubLists_collapse') != -1) {//collapse image
                                        if (panelsOnly == false) {
                                            if (showOrHide == 'hide') {
                                                //hide (collapse)
                                                //ShowOrHideInlineElement(currChild.id, false);
                                                //these images don't runat server so they all have the same id; function just updates 1st one always
                                                currChild.style.display = "none";
                                                //updated 3/12/2014 so it still retains space... specific to expand/collapse images and X's or remove/delete buttons
                                                //currChild.style.display = "hidden";
                                            } else {
                                                //show (expand)
                                                //ShowOrHideInlineElement(currChild.id, true);
                                                //these images don't runat server so they all have the same id; function just updates 1st one always
                                                currChild.style.display = "inline";
                                            }
                                        }
                                    } else if (currChild.id.indexOf('SubListsExpandedOrCollapsed') != -1) {//hidden field
                                        if (panelsOnly == false) {
                                            if (showOrHide == 'hide') {
                                                //hide (collapse)
                                                currChild.value = 'collapsed';
                                            } else {
                                                //show (expand)
                                                currChild.value = 'expanded';
                                            }
                                        }
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }
    }
}
//added 2/21/2014
function expandCollapseAllSublistsForElements(el, expandOrCollapse) {
    if (typeof (expandOrCollapse) == 'undefined') {
        expandOrCollapse = 'expand';
    }

    var els = document.getElementsByTagName(el);
    if (els) {
        for (var i = 0; i < els.length; i++) {
            //var subListsExpandCollapseSpan = GetChildSublistsExpandCollapseSpan(els[i]);
            //if (subListsExpandCollapseSpan) {
            //    //updated 3/13/2014 to ignore if expand/collapse span is hidden (wasn't previously being found because of server visible="False")
            //    //note 3/13/2014: can only do this if everything has an initial value (default to expanded... collapse image is visible)
            //    var okayToContinue = true;
            //    if (subListsExpandCollapseSpan.style) {
            //        if (subListsExpandCollapseSpan.style.visibility == "hidden" || subListsExpandCollapseSpan.style.display == "none") {
            //            okayToContinue = false;
            //        }
            //    }
            //    if (okayToContinue == true) { //added IF 3/13/2014; was previously happening every time
            //        //expandCollapseSubLists(subListsExpandCollapseSpan, expandOrCollapse);
            //        //this needs to get 1 of the child elements 1st
            //        var hdnSubListsExpandedOrCollapsed = GetChildSublistsExpandedOrCollapsedHiddenField(subListsExpandCollapseSpan);
            //        if (hdnSubListsExpandedOrCollapsed) {
            //            expandCollapseSubLists(hdnSubListsExpandedOrCollapsed, expandOrCollapse);
            //        }
            //    }
            //}
            //updated 3/19/2014 to use new reusable function
            expandCollapseChildSpanSublists(els[i], expandOrCollapse);
        }
    }
}
function GetChildSublistsExpandCollapseSpan(el) {
    if (el) {
        return GetChildField(el, 'SubLists_expandCollapseImageArea');
    }
    return null;
}
function RetainExpandedCollapsedSublistsForElements(el) {
    var els = document.getElementsByTagName(el);
    if (els) {
        for (var i = 0; i < els.length; i++) {
            //var hdnSubListsExpandedOrCollapsed = GetChildSublistsExpandedOrCollapsedHiddenField(els[i]);
            //if (hdnSubListsExpandedOrCollapsed) {
            //    if (hdnSubListsExpandedOrCollapsed.value == 'expanded') {
            //        expandCollapseSubLists(hdnSubListsExpandedOrCollapsed);
            //    } else if (hdnSubListsExpandedOrCollapsed.value == 'collapsed') {
            //        expandCollapseSubLists(hdnSubListsExpandedOrCollapsed, 'collapse');
            //    }
            //}
            //this need to 1st get the parent element
            var subListsExpandCollapseSpan = GetChildSublistsExpandCollapseSpan(els[i]);
            if (subListsExpandCollapseSpan) {
                //updated 3/20/2014 to add clickable class when expandCollapse section is visible
                var applyClickableClass = false;

                //updated 3/13/2014 to ignore if expand/collapse span is hidden (wasn't previously being found because of server visible="False")
                //note 3/13/2014: can only do this if everything has an initial value (default to expanded... collapse image is visible)
                //var okayToContinue = true;
                //if (subListsExpandCollapseSpan.style) {
                //    if (subListsExpandCollapseSpan.style.visibility == "hidden" || subListsExpandCollapseSpan.style.display == "none") {
                //        okayToContinue = false;
                //    }
                //}
                //if (okayToContinue == true) { //added IF 3/13/2014; was previously happening every time
                //updated 3/20/2014 to use new function
                if (isVisible(subListsExpandCollapseSpan) == true) {
                    applyClickableClass = true; //added 3/20/2014

                    var hdnSubListsExpandedOrCollapsed = GetChildSublistsExpandedOrCollapsedHiddenField(subListsExpandCollapseSpan);
                    if (hdnSubListsExpandedOrCollapsed) {
                        if (hdnSubListsExpandedOrCollapsed.value == 'expanded') {
                            expandCollapseSubLists(hdnSubListsExpandedOrCollapsed);
                        } else if (hdnSubListsExpandedOrCollapsed.value == 'collapsed') {
                            expandCollapseSubLists(hdnSubListsExpandedOrCollapsed, 'collapse');
                        }
                    }
                }
                //updated 3/20/2014; 3/21/2014 note: will need to remove logic if expand/collapse functionality is removed from header-click
                if (el == 'div') {
                    //var cls = els[i].className;
                    //if (applyClickableClass == true) {
                    //    if (cls.length > 0) {
                    //        cls = cls + ' ';
                    //    }
                    //    cls = cls + 'clickableHeader';
                    //} else {
                    //    if (cls.length > 0) {
                    //        if (cls == 'clickableHeader') {
                    //            cls = '';
                    //        } else {
                    //            cls = cls.replace(' clickableHeader', '');
                    //            cls = cls.replace('clickableHeader ', '');
                    //        }
                    //    }
                    //}
                    //els[i].className = cls;
                    ToggleCssClassForElement(els[i], 'clickableHeader', applyClickableClass);
                }
            }
        }
    }
}
function GetChildSublistsExpandedOrCollapsedHiddenField(el) {
    if (el) {
        return GetChildField(el, 'SubListsExpandedOrCollapsed');
    }
    return null;
}

//function ShowOrHideTableElement(elementId, show) {
//    if (typeof (show) == 'undefined') {
//        show = true;
//    }

//    if (show == true) {
//        if (document.getElementById(elementId)) {
//            //document.getElementById(elementId).style.visibility = "visible";
//            document.getElementById(elementId).style.display = "table";
//        }
//    } else {
//        if (document.getElementById(elementId)) {
//            //document.getElementById(elementId).style.visibility = "hidden";
//            document.getElementById(elementId).style.display = "none";
//        }
//    }
//}
//function ShowOrHideBlockElement(elementId, show) {
//    if (typeof (show) == 'undefined') {
//        show = true;
//    }

//    if (show == true) {
//        if (document.getElementById(elementId)) {
//            document.getElementById(elementId).style.display = "block";
//        }
//    } else {
//        if (document.getElementById(elementId)) {
//            document.getElementById(elementId).style.display = "none";
//        }
//    }
//}
//function ShowOrHideInlineElement(elementId, show) {
//    if (typeof (show) == 'undefined') {
//        show = true;
//    }

//    if (show == true) {
//        if (document.getElementById(elementId)) {
//            document.getElementById(elementId).style.display = "inline";
//        }
//    } else {
//        if (document.getElementById(elementId)) {
//            document.getElementById(elementId).style.display = "none";
//            //updated 3/12/2014 so it still retains space... specific to expand/collapse images and X's or remove/delete buttons
//            //document.getElementById(elementId).style.display = "hidden";
//        }
//    }
//}

//added 1/15/2014
function GetParentElementForChild(child) {
    if (child) {
        var parent = child.parentNode;
        if (parent) {
            return parent;
        }
    }
    return null;
}
function ClickRelatedChildButton(child, relatedButtonId, checkEditMode) {
    //3/4/2014 - added optional parameter
    if (typeof (checkEditMode) == 'undefined') {
        checkEditMode = true;
    }

    //added 5/15/2014
    //var parent = GetParentElementForChild(child);
    //if (parent) {
    //    if (IsTextInWord(parent.className, 'disabledSectionHeader') == true) {
    //        //alert('You cannot access this feature right now.');
    //        return;
    //    }
    //}
    if (HasAnyDisabledParentListItem(child) == true) {
        return;
    }

    //added 3/10/2014
    //SetClickedElement(child);
    //updated 3/11/2014
    SetClickedElementWithRelatedChildButtonId(child, relatedButtonId);

    //updated for edit mode 1/22/2014; updated for optional parameter 3/4/2014
    if (checkEditMode == true) {
        if (InEditMode(true) == true) {
            //alert("This functionality is currently locked.");
            return;
        }
    }

    if (child && relatedButtonId) {
        if (relatedButtonId.length > 0) {
            var relatedButton = GetRelatedChildField(child, relatedButtonId);
            if (relatedButton) {
                relatedButton.click();
            }
        }
    }
}
//added 6/3/2014; originally copied from ClickRelatedChildButton
function ClickRelatedChildButtonFromSummaryHeader(summaryHeader, relatedButtonId, checkEditMode) {
    if (typeof (checkEditMode) == 'undefined') {
        checkEditMode = true;
    }

    if (HasAnyDisabledParentListItem(summaryHeader) == true) {
        return;
    }

    var okayToContinue = true;
    var sectionHeader = GetParentElementForChild(summaryHeader);
    if (sectionHeader) {
        var hdnSuccessfullyRatedFlag = GetRelatedChildField(sectionHeader, 'SummarySection_SuccessfullyRatedFlag');
        if (hdnSuccessfullyRatedFlag) {
            if (hdnSuccessfullyRatedFlag.value == 'true') {
                //clickable header is enabled; function as normal
            } else {
                //clickable header is disabled
                okayToContinue = false;
            }
        }
    }

    if (okayToContinue == true) {
        SetClickedElementWithRelatedChildButtonId(summaryHeader, relatedButtonId);

        if (checkEditMode == true) {
            if (InEditMode(true) == true) {
                return;
            }
        }

        if (sectionHeader && relatedButtonId) {
            if (relatedButtonId.length > 0) {
                var relatedButton = GetChildField(sectionHeader, relatedButtonId);
                if (relatedButton) {
                    relatedButton.click();
                }
            }
        }
    }
}
//added 2/25/2014
function ClickChildButtonOfRelatedChildElement(currElement, relatedElementId, childButtonId, checkEditMode) {
    //3/19/2014 - added optional parameter
    if (typeof (checkEditMode) == 'undefined') {
        checkEditMode = true;
    }

    //updated for optional parameter 3/19/2014
    if (checkEditMode == true) {
        if (InEditMode(true) == true) {
            return;
        }
    }

    if (currElement && relatedElementId && childButtonId) {
        if (relatedElementId.length > 0) {
            var relatedElement = GetRelatedChildField(currElement, relatedElementId);
            if (relatedElement) {
                ClickChildButton(relatedElement, childButtonId, checkEditMode); //added optional parameter 3/19/2014
            }
        }
    }
}
function ClickChildButton(parent, childButtonId, checkEditMode) {
    //3/19/2014 - added optional parameter
    if (typeof (checkEditMode) == 'undefined') {
        checkEditMode = true;
    }

    //updated for optional parameter 3/19/2014
    if (checkEditMode == true) {
        if (InEditMode(true) == true) {
            return;
        }
    }

    if (parent && childButtonId) {
        if (childButtonId.length > 0) {
            var childButton = GetChildField(parent, childButtonId);
            if (childButton) {
                childButton.click();
            }
        }
    }
}
function ToggleTitleAndClickableClass(el, onFlag, title) {
    if (el) {
        if (typeof (onFlag) == 'undefined') {
            onFlag = true;
        }
        if (onFlag == true) {
            if (typeof (title) == 'undefined') {
                if (el.title.length > 0) {
                    title = el.title;
                } else {
                    title = 'clickable';
                }
            }
            //el.className = 'clickable';
            //el.className = 'clickableHeader';//updated 2/26/2014; removed 3/20/2014... now being done after IF w/ ToggleCssClassForElement
            el.title = title;
        } else {
            //el.className = ''; //removed 3/20/2014... now being done after IF w/ ToggleCssClassForElement
            el.title = '';
        }
        ToggleCssClassForElement(el, 'clickableHeader', onFlag); //updated 3/20/2014 to use this
        var hdnTitleFlag = GetChildHiddenTitleFlag(el);
        if (hdnTitleFlag) {
            if (onFlag == true) {
                hdnTitleFlag.value = 'on';
            } else {
                hdnTitleFlag.value = 'off';
            }
        }
    }
}
function ToggleTitleAndClickableClassForRelatedParentSibling(el, onFlag, title) {
    if (el) {
        var parent = GetParentElementForChild(el);
        if (parent) {
            //alert('parent id: ' + parent.id);
            //var headerSpan = GetChildHeaderSpan(parent);
            var headerSpan = GetRelatedChildHeaderSpan(parent);
            if (headerSpan) {
                //alert('header span id: ' + headerSpan.id);
                ToggleTitleAndClickableClass(headerSpan, onFlag, title);
            }
        }
    }
}
function GetRelatedChildHeaderSpan(el) {
    if (el) {
        var parent = GetParentElementForChild(el);
        if (parent) {
            return GetChildHeaderSpan(parent);
        }
    }
}
function GetChildHeaderSpan(el) {
    if (el) {
        //return GetChildField(el, 'Header');
        //updated 1/7/2015 to use new function w/ optional param... was picking up a different element w/ Header in the id
        return GetChildField(el, 'Header', 'span');
    }
    return null;
}
function GetChildHiddenTitleFlag(el) {
    if (el) {
        return GetChildField(el, 'hdnTitleFlag_');
    }
    return null;
}
function RetainTitleAndClickableClassForSpans() {
    var spans = document.getElementsByTagName('span');
    if (spans) {
        for (var i = 0; i < spans.length; i++) {
            if (spans[i].id.length > 0) {
                if (spans[i].id.indexOf('Header') != -1) {
                    var hdnTitleFlag = GetChildHiddenTitleFlag(spans[i]);
                    if (hdnTitleFlag) {
                        if (hdnTitleFlag.value == 'on') {
                            //turn on (shouldn't need to set title since it will already have a value)
                            ToggleTitleAndClickableClass(spans[i], true);
                        } else if (hdnTitleFlag.value == 'off') {
                            //turn off... can't just do on else because it will clear everything out on 1st load
                            ToggleTitleAndClickableClass(spans[i], false);
                        }
                    }
                }
            }
        }
    }
}

//added 1/22/2014
function ChangeClassNameForElements(el, OldClass, NewClass, removeSelectedFlag) {
    if (typeof (removeSelectedFlag) == 'undefined') {
        removeSelectedFlag = false;
    }

    var els = document.getElementsByTagName(el);
    if (els) {
        for (var i = 0; i < els.length; i++) {
            if (els[i].className == OldClass) {
                els[i].className = NewClass; //note 3/20/2014: could utilize new ToggleCssClassForElement function
                if (removeSelectedFlag == true) {
                    var hiddenSelectedField = GetChildHiddenSelectedField(els[i]);
                    if (hiddenSelectedField) {
                        hiddenSelectedField.value = '';
                    }
                }
            }
        }
    }
}
function AssignSelectedClassToElement(el) {
    if (el) {
        RemoveSelectedClassFromAllListItems();
        el.className = 'selected'; //note 3/20/2014: could utilize new ToggleCssClassForElement function
        var hiddenSelectedField = GetChildHiddenSelectedField(el);
        if (hiddenSelectedField) {
            hiddenSelectedField.value = 'yes';
        }
    }
}
function RemoveSelectedClassFromAllListItems() {
    RetainSelectedListItems(); //needed to add this 1/24/2014 because page doesn't hold the element classNames between postbacks
    ChangeClassNameForElements('li', 'selected', '', true);
    //note 3/20/2014: could utilize new ToggleCssClassForElement function
}
function SelectParentListItem(child) {
    //can set optional param to true to show error message... InEditMode(true)
    if (InEditMode() == true) {
        return;
    }

    if (child) {
        var parent = GetParentElementForChild(child);
        if (parent) {
            if (parent.localName == 'li') {
                AssignSelectedClassToElement(parent);
            } else {
                SelectParentListItem(parent);
                return;
            }
        }
    }
}
function GetChildHiddenSelectedField(el) {
    if (el) {
        return GetChildField(el, 'hdnIsSelected');
    }
    return null;
}
function CheckForSelected() {
    var hdnDeselectAllListItemsFlag = document.getElementById(hdnDeselectAllListItemsFlagId);
    if (hdnDeselectAllListItemsFlag) {
        //if (hdnDeselectAllListItemsFlag.value == 'yes') {
        if (hdnDeselectAllListItemsFlag.value == 'yes' || InEditMode() == false) {
            RemoveSelectedClassFromAllListItems();
            hdnDeselectAllListItemsFlag.value = '';
            //document.getElementById(hdnDeselectAllListItemsFlagId).value = '';
            return;
        }
        //RetainSelectedListItems(); //1/22/2014 note: would probably be better after next } since it's not dependent on IF
    } else if (InEditMode() == false) {
        RemoveSelectedClassFromAllListItems();
        return;
    }
    RetainSelectedListItems();
}
function RetainSelectedListItems() {
    var els = document.getElementsByTagName('li');
    if (els) {
        for (var i = 0; i < els.length; i++) {
            var hiddenSelectedField = GetChildHiddenSelectedField(els[i]);
            if (hiddenSelectedField) {
                //note 3/20/2014: could utilize new ToggleCssClassForElement function
                if (hiddenSelectedField.value == 'yes') {
                    els[i].className = 'selected'
                } else {
                    els[i].className = ''
                }
            }
        }
    }
}
//function RetainEnabledOrDisabledFlagsForSectionHeaders() { //added 5/15/2014
//    var els = document.getElementsByTagName('div');
//    if (els) {
//        for (var i = 0; i < els.length; i++) {
//            //if (els[i].className.toUpperCase().indexOf('SECTIONHEADER') != -1) {
//            if (IsSectionHeader(els[i]) == true) {
//                var hdnEnabledOrDisabledFlag = GetChildField(els[i], '_EnabledOrDisabledFlag');
//                if (hdnEnabledOrDisabledFlag) {
//                    if (hdnEnabledOrDisabledFlag.value == 'disabled') {
//                        ReplaceClassNameForElement(els[i], 'sectionHeader', 'disabledSectionHeader');
//                        EnableDisableParentListItem(els[i], true);
//                    } else { //assume it's enabled
//                        ReplaceClassNameForElement(els[i], 'disabledSectionHeader', 'sectionHeader');
//                        EnableDisableParentListItem(els[i], false);
//                    }
//                }
//            }
//        }
//    }
//}
function RetainEnabledOrDisabledFlagsForParentListItemOfSectionHeaders() { //added 5/15/2014
    var els = document.getElementsByTagName('div');
    if (els) {
        for (var i = 0; i < els.length; i++) {
            //if (els[i].className.toUpperCase().indexOf('SECTIONHEADER') != -1) {
            if (IsSectionHeader(els[i]) == true) {
                //var hdnEnabledOrDisabledFlag = GetChildField(els[i], '_EnabledOrDisabledFlag');
                //if (hdnEnabledOrDisabledFlag) {
                //    if (hdnEnabledOrDisabledFlag.value == 'disabled') {
                //        ReplaceClassNameForElement(els[i], 'sectionHeader', 'disabledSectionHeader');
                //        EnableDisableParentListItem(els[i], true);
                //    } else { //assume it's enabled
                //        ReplaceClassNameForElement(els[i], 'disabledSectionHeader', 'sectionHeader');
                //        EnableDisableParentListItem(els[i], false);
                //    }
                //}
                var parentListItem = GetParentListItem(els[i]);
                if (parentListItem) {
                    var hdnEnabledOrDisabledFlag = GetChildField(parentListItem, '_EnabledOrDisabledFlag');
                    if (hdnEnabledOrDisabledFlag) {
                        if (hdnEnabledOrDisabledFlag.value == 'disabled') {
                            AddRemoveDisabledTreeClass(parentListItem, true);
                        } else { //assume it's enabled
                            AddRemoveDisabledTreeClass(parentListItem, false);
                        }
                    }
                }
            }
        }
    }
}
function EnableDisableParentListItem(element, addDisabledClass) { //added 5/15/2014
    if (element) {
        //var parent = GetParentElementForChild(element);
        //if (parent) {
        //    if (parent.tagName.toUpperCase() == 'LI') {
        //        if (typeof (addDisabledClass) == 'undefined') {
        //            addDisabledClass = false;
        //        }
        //        ToggleCssClassForElement(parent, 'disabledTree', addDisabledClass)
        //    }
        //}
        var parentListItem = GetParentListItem(element);
        if (parentListItem) {
            if (typeof (addDisabledClass) == 'undefined') {
                addDisabledClass = false;
            }
            ToggleCssClassForElement(parentListItem, 'disabledTree', addDisabledClass)
        }
    }
}
function GetParentListItem(element) { //added 5/15/2014
    if (element) {
        var parent = GetParentElementForChild(element);
        if (parent) {
            if (parent.tagName) {
                if (parent.tagName.toUpperCase() == 'LI') {
                    return parent;
                } else {
                    return GetParentElementForChild(parent);
                }
            }
        }
    }
    return null;
}
function IsParentListItemDisabled(element) {
    if (element) {
        var parentListItem = GetParentListItem(element);
        if (parentListItem) {
            //return IsTextInWord(parentListItem.className, 'disabledTree');
            return HasDisabledTreeClass(parentListItem);
        }
    }
    return false;
}
function HasAnyDisabledParentListItem(element) {
    if (element) {
        var parentListItem = GetParentListItem(element);
        if (parentListItem) {
            if (HasDisabledTreeClass(parentListItem) == true) {
                return true;
            } else {
                return HasAnyDisabledParentListItem(parentListItem);
            }
        }
    }
    return false;
}
function HasDisabledTreeClass(element) {
    if (element) {
        return IsTextInWord(element.className, 'disabledTree');
    }
    return false;
}
function AddRemoveDisabledTreeClass(element, addDisabledClass) {
    if (typeof (addDisabledClass) == 'undefined') {
        addDisabledClass = false;
    }
    ToggleCssClassForElement(element, 'disabledTree', addDisabledClass)
}
function ReplaceClassNameForElement(el, oldClassName, newClassName) { //added 5/15/2014
    if (el && oldClassName && newClassName) {
        if (oldClassName.length > 0 && newClassName.length > 0) {
            //if (el.className.indexOf(oldClassName) != -1) {
            if (IsTextInWord(el.className, oldClassName) == true) { //this will only work correctly if not matching casing... since the text sectionHeader is in disabledSectionHeader
                //old class is there
                if (IsTextInWord(el.className, newClassName) == false) { //this will only work correctly if not matching casing... since the text sectionHeader is in disabledSectionHeader
                    //new class isn't there
                    var cls = el.className;

                    //if (cls == oldClassName) {
                    //    cls = newClassName;
                    //} else {
                    //    cls = cls.replace(' ' + oldClassName, ' ' + newClassName); //in case it's after another class
                    //    cls = cls.replace(oldClassName + ' ', newClassName + ' '); //in case it's before another class
                    //}
                    cls = cls.replace(oldClassName, newClassName); //this will only work correctly if not matching casing... since the text sectionHeader is in disabledSectionHeader

                    el.className = cls;
                }
            }
            //might be better to use ToggleCssClassForElement... 1st to remove old class and 2nd to add new class... might still have to make sure old class is there when removing and new class isn't already there to prevent any issues w/ the text sectionHeader being in disabledSectionHeader
        }
    }
}
function IsSectionHeader(el) { //added 5/15/2014
    if (el) {
        //if (el.className.toUpperCase().indexOf('SECTIONHEADER') != -1) {
        if (IsTextInWord(el.className, 'SECTIONHEADER', true) == true) {
            return true;
        }
    }

    return false;
}
function IsTextInWord(strWord, strText, ignoreCasing) { //added 5/15/2014
    if (strWord && strText) {
        if (strWord.length > 0 && strText.length > 0) {
            if (typeof (ignoreCasing) == 'undefined') {
                ignoreCasing = false;
            }
            if (ignoreCasing == true) {
                strWord = strWord.toUpperCase();
                strText = strText.toUpperCase();
            }
            if (strWord.indexOf(strText) != -1) {
                return true;
            }
        }
    }

    return false;
}
function InitializeTree() { //changed from InitializePage 5/15/2014
    //RetainEnabledOrDisabledFlagsForSectionHeaders(); //added 5/15/2014; needs to happen before CheckForExpandOrCollapse in case it needs to only apply clickable class whenever sectionHeader is enabled; since changed
    CheckForExpandOrCollapse();
    CheckForSelected();
    RetainTitleAndClickableClassForSpans();//added 2/25/2014
    RetainEditAndViewSectionDisplaysForSpans(); //added 3/5/2014
    //RetainEnabledOrDisabledFlagsForSectionHeaders(); //added 5/15/2014; now needs to happen after CheckForExpandOrCollapse since expandCollapseSubLists won't work if disabledTree class is applied to parent LI
    RetainEnabledOrDisabledFlagsForParentListItemOfSectionHeaders(); //added 5/15/2014... changed functionality to work on parent li tags instead of section headers... so class would apply to entire section
}
var aboutToSave = false; //added 3/17/2014
function InEditMode(showError) {
    if (typeof (showError) == 'undefined') {
        showError = false;
    }

    //alert('InEditMode check');

    //added 3/10/2014
    suppressedEditModeMessageOnClick = 'no';

    var hdnInEditModeFlag = document.getElementById(hdnInEditModeFlagId);
    if (hdnInEditModeFlag) {
        if (hdnInEditModeFlag.value == 'true') {//changed from yes to true 1/24/2014
            //updated 3/10/2014 to always hide message in certain scenarios (when calendar is open)... when functionality will be attempted again after edit mode is turned off
            //note 3/17/2014: for quoteDesc and effDate, still get edit mode message when onblur happens whenever save is about to happen in code-behind (doesn't happen when calendar is open)
            if (clickedElement) {
                //see if calendar is open
                var bdp = document.getElementById(dbpEffectiveDateClientId);
                if (bdp) {
                    var hdnIsVisible_BdpEffectiveDateCalendar = GetRelatedChildField(bdp, 'hdnIsVisible_BdpEffectiveDateCalendar');
                    if (hdnIsVisible_BdpEffectiveDateCalendar) {
                        if (hdnIsVisible_BdpEffectiveDateCalendar.value == 'yes') {
                            suppressedEditModeMessageOnClick = 'yes';
                            showError = false;
                            //alert('suppressedEditModeMessageOnClick');
                        }
                    }
                }
            }
            if (showError == true) {
                if (aboutToSave == false) { //added 3/17/2014; was previously showing alert all the time
                    alert("This functionality is currently locked.");
                }
            }
            return true;
        }
    }
    return false;
}

var ignoreFurtherToggles = false;/*matt 2-5-14*/
function ToggleEditMode(onFlag, _ignoreFurtherToggles) {
    if (typeof (onFlag) == 'undefined') {
        onFlag = false;
    }
    if (ignoreFurtherToggles == false) {/*matt 2-5-14*/
        SwitchEditMode(onFlag);
    }

    DoEditModeOverlay();

    if (typeof (_ignoreFurtherToggles) != 'undefined') { /*Allow to ste this time but might cause future calls to be ignored  matt 2-5-14*/
        ignoreFurtherToggles = _ignoreFurtherToggles;
    }
}
//added 3/6/2014 to replicate original ToggleEditMode functionality... w/o the overlay
function SwitchEditMode(onFlag) {
    if (typeof (onFlag) == 'undefined') {
        onFlag = false;
    }
    var hdnInEditModeFlag = document.getElementById(hdnInEditModeFlagId);
    if (hdnInEditModeFlag) {
        if (onFlag == true) {
            hdnInEditModeFlag.value = 'true';
        } else {
            hdnInEditModeFlag.value = 'false';
        }
    }
}

function DoEditModeOverlay() {
    var hdnInEditModeFlag = document.getElementById(hdnInEditModeFlagId);

    var $somediv = $('#' + pnlTreeViewClientId),
    pos = $.extend({
        width: $somediv.outerWidth(),
        height: $somediv.outerHeight()
    }, $somediv.position());

    if (hdnInEditModeFlag.value == 'true') {
        // if overlay does not exist already
        $("#liRecentQuoteActivity").hide(); // Matt A - 11-1-14
        if ($("#div_Overlay_Tree").length == 0) {
            // create new div over tree to stop button clicks
            $('<div>', {
                id: 'div_Overlay_Tree',
                title: 'Tree is disabled until the current activity is saved.',
                css: {
                    position: 'absolute',
                    top: pos.top,
                    left: pos.left,
                    width: pos.width,
                    height: pos.height,
                    backgroundColor: '#000'
                    , opacity: 0.0
                }
            }).appendTo($somediv);
            //$("#div_Overlay_Tree").mouseenter(function () {
            //    $("#div_Overlay_Tree").animate({ opacity: 0.25 });
            //});
            //$("#div_Overlay_Tree").mouseleave(function () {
            //    $("#div_Overlay_Tree").animate({ opacity: 0.0 });
            //});
        }
        $("#" + pnlTreeViewClientId).addClass('disabledTree'); /*matt 2-5-14*/
    }
    else {
        $("#liRecentQuoteActivity").show();// Matt A - 11-1-14
        $("#div_Overlay_Tree").remove();
        $("#" + pnlTreeViewClientId).removeClass('disabledTree'); /*matt 2-5-14*/
    }
}

//added 3/4/2014
function OnBlurQuoteDescriptionOrEffDate(field) {
    //var test = event.srcElement.name;//just gives element w/ onblur event (i.e. for effDate: ctl100$cphMain$ctlTreeView$bdpEffectiveDate$TextBox)
    //if (test) {
    //    alert('onblur cause: ' + test);
    //}
    //var test = event.target;//event.target.name gets undefined error
    //if (test) {
    //    alert('onblur cause: ' + test);
    //}
    if (field) {
        var parent = GetParentElementForChild(field); //EditSection; should be at same level as hidden field
        if (parent) {
            //alert('parent id: ' + parent.id);
            //see if field is child textbox of Basic Date Picker
            var checkDate = false;
            var fieldForButtonClick = field;
            var parentForHiddenField = parent;
            if (field.id.length > 0) {
                //alert('field id: ' + field.id);
                if (field.id.indexOf('bdpEffectiveDate') != -1) {
                    checkDate = true;

                    //test code
                    //var selectedDate = fieldForButtonClick.getSelectedDate();//BDP js function
                    //var selectedDate = getSelectedDate(fieldForButtonClick.id);//BDP js function
                    //alert('selected date: ' + selectedDate);

                    if (parent.id.length > 0) {
                        if (parent.id.indexOf('bdpEffectiveDate') != -1) {
                            var parent2 = GetParentElementForChild(parent);
                            if (parent2) {
                                //alert('parent2 id: ' + parent2.id);
                                fieldForButtonClick = parent;
                                parentForHiddenField = parent2;
                            }
                        }
                    }
                }
            }
            var hdnOriginalField = GetRelatedChildField(parentForHiddenField, 'hdnOriginal');
            if (hdnOriginalField) {
                //alert('hidden field id: ' + hdnOriginalField.id);
                //if (field.value == hdnOriginalField.value) {
                if (isMatch(field, hdnOriginalField, checkDate) == true) {
                    //same; cancel
                    ////ClickRelatedChildButton(fieldForButtonClick, 'imgBtnCancelSave', false); //false tells it to ignore Edit mode
                    ////updated 3/5/2014 to do what button click was doing so there's no postback (javascript and code-behind)
                    CancelQuoteDescriptionOrEffectiveDateEdit(parentForHiddenField, hdnOriginalField, checkDate, fieldForButtonClick);
                } else {
                    //changed; save
                    //added validation 3/7/2014
                    if (field.value.trim().length == 0) {
                        //has nothing
                        //field.focus();//moved 3/11/2014 for confirm
                        var missingTextMsg;//added 3/11/2014
                        if (checkDate == true) {
                            //alert('please enter the effective date');
                            //updated 3/11/2014 for confirm
                            missingTextMsg = 'please enter the effective date';
                        } else {
                            //alert('please enter the quote description');
                            //updated 3/11/2014 for confirm
                            //missingTextMsg = 'please enter the quote description';
                            //updated 7/5/2019
                            var isRemarks = false;
                            //var lblDescriptionOrRemarks = GetRelatedChildField(field, 'lblDescriptionOrRemarks');
                            //var lblDescriptionOrRemarks = GetParentSibling(field, 'lblDescriptionOrRemarks');
                            //var lblDescriptionOrRemarks = GetCousinField(field, 'QuoteDescriptionHeader', 'lblDescriptionOrRemarks');
                            //var DescriptionOrRemarksText = GetCousinField(field, 'QuoteDescriptionHeader', 'DescriptionOrRemarksText');
                            var DescriptionOrRemarksText = GetParentSiblingGrandchildField(field, 'QuoteDescriptionHeader', 'DescriptionOrRemarksBoldTag', 'DescriptionOrRemarksText');
                            if (DescriptionOrRemarksText) {
                                if (DescriptionOrRemarksText.innerHTML == 'Remarks') {
                                    isRemarks = true;
                                }
                            }

                            if (isRemarks == true) {
                                missingTextMsg = 'please enter the remarks';
                            } else {
                                missingTextMsg = 'please enter the quote description';
                            }
                        }
                        var okayToContinue = confirm(missingTextMsg);
                        if (okayToContinue == true) {
                            field.focus();
                        } else {
                            CancelQuoteDescriptionOrEffectiveDateEdit(parentForHiddenField, hdnOriginalField, checkDate, fieldForButtonClick);
                        }
                        return;
                    } else {
                        //has something
                        if (checkDate == true) {
                            var testDt = new Date(field.value);
                            if (isDate(testDt) == false) {
                                //field.focus();//moved 3/11/2014 for confirm
                                var validDateMsg = 'please enter a valid date';
                                var strDateFormat = GetDateFormatForDatePickerElement(fieldForButtonClick);
                                if (strDateFormat) {
                                    if (strDateFormat.length > 0) {
                                        validDateMsg = validDateMsg + ' in the following format: ' + strDateFormat;
                                    }
                                }
                                //alert(validDateMsg);
                                var okayToContinue = confirm(validDateMsg);
                                if (okayToContinue == true) {
                                    field.focus();
                                } else {
                                    CancelQuoteDescriptionOrEffectiveDateEdit(parentForHiddenField, hdnOriginalField, checkDate, fieldForButtonClick);
                                }
                                return;
                            } else {
                                //valid date

                                //updated 9/12/2015
                                var isValidDate = true;
                                var validDateMsg = 'please enter a valid date';
                                var minEffDate = MinimumEffectiveDate(parentForHiddenField);
                                var maxEffDate = MaximumEffectiveDate(parentForHiddenField);
                                if (minEffDate && maxEffDate) {
                                    validDateMsg = 'effective date must be between ' + minEffDate + ' and ' + maxEffDate;
                                    if (isDateBetweenMinimumAndMaximumDates(field.value, minEffDate, maxEffDate) == true) {
                                        isValidDate = true; //redundant
                                    } else {
                                        //quote date is not between minDate and maxDate
                                        isValidDate = false;
                                        //var minEffDateAllQuote = MinimumEffectiveDateAllQuotes();
                                        //var minQuoteDateIsGreaterThanAllDate = MinimumQuoteEffectiveDateIsGreaterThanDateForAllQuotes();
                                        if (isDate1LessThanDate2(field.value, minEffDate) == true) {
                                            var updateToMinDate = false;
                                            if (MinimumQuoteEffectiveDateIsGreaterThanDateForAllQuotes(parentForHiddenField) == true) {
                                                updateToMinDate = true;                                                
                                            } else if (minEffDate == MinimumEffectiveDateAllQuotes(parentForHiddenField) && QuoteHasMinimumEffectiveDate(parentForHiddenField) == true) {
                                                updateToMinDate = true;
                                            } else {
                                                updateToMinDate = false; //redundant
                                            }
                                            if (updateToMinDate == true) {
                                                if (isDate1EqualToDate2(hdnOriginalField.value, minEffDate) == true) {
                                                    alert("Contact Underwriting for quotes before " + minEffDate + "; date reverted back to " + minEffDate + ".");
                                                    CancelQuoteDescriptionOrEffectiveDateEdit(parentForHiddenField, hdnOriginalField, checkDate, fieldForButtonClick);
                                                    return;
                                                } else {
                                                    isValidDate = true;
                                                    field.value = minEffDate;
                                                    alert("Contact Underwriting for quotes before " + minEffDate + "; date set to " + minEffDate + ".");
                                                }                                                
                                            } else {
                                                //quote date is less than minDate
                                                //can customize message but already defaulted above
                                                var beforeDtMsg = BeforeDateMsg(parentForHiddenField);
                                                if (StringHasSomething(beforeDtMsg) == true) {
                                                    validDateMsg = appendText(validDateMsg, beforeDtMsg, "\n\n");
                                                }
                                            }
                                        } else if (isDate1GreaterThanDate2(field.value, maxEffDate) == true) {
                                            //quote date is more than maxDate
                                            //can customize message but already defaulted above
                                            var afterDtMsg = AfterDateMsg(parentForHiddenField);
                                            if (StringHasSomething(afterDtMsg) == true) {
                                                validDateMsg = appendText(validDateMsg, afterDtMsg, "\n\n");
                                            }
                                        }else{
                                            //shouldn't get here
                                            //can customize message but already defaulted above
                                        }
                                    }
                                }

                                if (isValidDate == true) { //added IF 9/12/2015
                                    //setSelectedDate(testDt, fieldForButtonClick.id);//doesn't work... can't access BDP js api from outside of one of their methods
                                    //if (fieldForButtonClick.innerText.length > 0) {
                                    //    var dateFormatIndex = fieldForButtonClick.innerText.indexOf('dateFormat:"');
                                    //    if (dateFormatIndex != -1) {
                                    //        var strDateFormat = fieldForButtonClick.innerText.substr(dateFormatIndex, fieldForButtonClick.innerText.length - dateFormatIndex - 12); //length of 'dateFormat:"' = 12
                                    //        if (strDateFormat.length > 0){
                                    //            var closingQuoteIndex = strDateFormat.indexOf('"');
                                    //            if (closingQuoteIndex != -1) {
                                    //                strDateFormat = strDateFormat.substr(0, closingQuoteIndex);
                                    //                if (strDateFormat.length > 0) {
                                    //                }
                                    //            }
                                    //        }
                                    //    }
                                    //}
                                    //var strDateFormat = GetDateFormatForDatePickerElement(fieldForButtonClick);
                                    //might try to format accordingly
                                    var strDateFormat = GetDateFormatForDatePickerElement(fieldForButtonClick);
                                    if (strDateFormat) {
                                        if (strDateFormat.length > 0) {
                                            field.value = FormatDateString(field.value, strDateFormat);
                                        }
                                    }
                                } else { //added 9/12/2015
                                    var okayToContinue = confirm(validDateMsg);
                                    if (okayToContinue == true) {
                                        field.focus();
                                    } else {
                                        CancelQuoteDescriptionOrEffectiveDateEdit(parentForHiddenField, hdnOriginalField, checkDate, fieldForButtonClick);
                                    }
                                    return;
                                }
                            }
                        }
                    }
                    //alert('going to save...');
                    aboutToSave = true; //added 3/17/2014 to suppress any edit mode messages that may be encountered
                    ClickRelatedChildButton(fieldForButtonClick, 'imgBtnSave', false); //false tells it to ignore Edit mode
                }
            }
        }
    }
}
//added 9/12/2015
function MinimumEffectiveDate(dtSibling) {
    return dateValueForRelatedChildField(dtSibling, 'hdnMinimumEffectiveDate');
}
function MaximumEffectiveDate(dtSibling) {
    return dateValueForRelatedChildField(dtSibling, 'hdnMaximumEffectiveDate');
}
function MinimumEffectiveDateAllQuotes(dtSibling) {

    return dateValueForRelatedChildField(dtSibling, 'hdnMinimumEffectiveDateAllQuotes');
}
function MaximumEffectiveDateAllQuotes(dtSibling) {
    return dateValueForRelatedChildField(dtSibling, 'hdnMaximumEffectiveDateAllQuotes');
}
function QuoteHasMinimumEffectiveDate(dtSibling) {
    var hdnQuoteHasMinimumEffectiveDate = GetRelatedChildField(dtSibling, 'hdnQuoteHasMinimumEffectiveDate');
    if (hdnQuoteHasMinimumEffectiveDate) {
        return strToBool(hdnQuoteHasMinimumEffectiveDate.value);
    }
    return false;
}
function MinimumQuoteEffectiveDateIsGreaterThanDateForAllQuotes(dtSibling) {
    var hdnMinimumQuoteEffectiveDateIsGreaterThanDateForAllQuotes = GetRelatedChildField(dtSibling, 'hdnMinimumQuoteEffectiveDateIsGreaterThanDateForAllQuotes');
    if (hdnMinimumQuoteEffectiveDateIsGreaterThanDateForAllQuotes) {
        return strToBool(hdnMinimumQuoteEffectiveDateIsGreaterThanDateForAllQuotes.value);
    }
    return false;
}
function BeforeDateMsg(dtSibling) {
    var hdnBeforeDateMsg = GetRelatedChildField(dtSibling, 'hdnBeforeDateMsg');
    if (hdnBeforeDateMsg) {
        return hdnBeforeDateMsg.value;
    }
    return false;
}
function AfterDateMsg(dtSibling) {
    var hdnAfterDateMsg = GetRelatedChildField(dtSibling, 'hdnAfterDateMsg');
    if (hdnAfterDateMsg) {
        return hdnAfterDateMsg.value;
    }
    return false;
}
function dateValueForField(fieldId) {
    if (fieldId) {
        if (fieldId.length > 0) {
            var dt = document.getElementById(fieldId);
            if (dt) {
                if (isStringDate(dt.value) == true) {
                    return dt.value;
                }
            }
        }
    }
    return null;
}
function dateValueForRelatedChildField(sibling, fieldId) {
    if (sibling && fieldId) {
        if (fieldId.length > 0) {
            var dt = GetRelatedChildField(sibling, fieldId);
            if (dt) {
                if (isStringDate(dt.value) == true) {
                    return dt.value;
                }
            }
        }
    }
    return null;
}
function dateValueForJQueryFieldSelector(fieldSelector) { //added 9/17/2015 for effDate validation from App Submit
    if (fieldSelector) {
        if (fieldSelector.length > 0) {
            var dt = $(fieldSelector);
            if (dt) {
                if (isStringDate(dt.val()) == true) {
                    return dt.val();
                }
            }
        }
    }
    return null;
}
function strToBool(str) {
    if (str) {
        if (str.length > 0) {
            var ucaseStr = str.toUpperCase();
            if (ucaseStr == 'TRUE' || ucaseStr == '1' || ucaseStr == 'YES') {
                return true;
            }
        }
    }
    return false;
}
function boolValueForJQueryFieldSelector(fieldSelector) { //added 9/18/2015
    if (fieldSelector) {
        if (fieldSelector.length > 0) {
            var dt = $(fieldSelector);
            if (dt) {
                return strToBool(dt.val());
            }
        }
    }
    return false;
}
function GetRelatedChildField(child, relatedFieldId) {
    //if (InEditMode(true) == true) {
    //    //alert("This functionality is currently locked.");
    //    return;
    //}

    if (child && relatedFieldId) {
        if (relatedFieldId.length > 0) {
            var parent = GetParentElementForChild(child);
            if (parent) {
                return GetChildField(parent, relatedFieldId);
            }
        }
    }
    return null; //added 3/5/2014
}
function isDate(x) { //9/17/2015 note: this appears to accept 4-digit numbers... 4444 comes back as 12/31/4443... typeof x.getDate() = "number"
    //return (null != x) && !isNaN(x) && ("undefined" !== typeof x.getDate);
    return (null != x) && !isNaN(x) && ("undefined" !== typeof x.getDate());
}
function isStringDate(str) { //added 9/12/2015
    if (str) {
        if (str.length > 0) {
            var dt = new Date(str);
            return isDate(dt);
        }
    }
    return false;
}
function isMatch(a, b, checkDate) {
    if (a && b) {
        if (typeof (checkDate) == 'undefined') {
            checkDate = false;
        }
        if (a.value == b.value) {
            return true;
        } else {
            //alert('a value: ' + a.value + '; b value: ' + b.value);
            if (checkDate == true) {
                //alert('checkDate = true');
                //alert('a.getDate: ' + a.getDate() + '; b.getDate: ' + b.getDate());
                a = new Date(a.value);
                //if (isDate(a.value) == true) {
                if (isDate(a) == true) {
                    //a is date
                    //alert('a is date');
                    b = new Date(b.value);
                    //if (isDate(b.value) == true) {
                    if (isDate(b) == true) {
                        //a and b are both dates
                        //alert('a and b are dates... a.getDate: ' + a.getDate() + '; b.getDate: ' + b.getDate());
                        //if (a.getDate() == b.getDate()) {
                        //getDate() just returns day of the month
                        if (a.getDate() == b.getDate() && a.getMonth() == b.getMonth() && a.getFullYear() == b.getFullYear()) {
                            return true;
                        }
                    }
                }
            }
        }
    }
    return false;
}
//added 9/12/2015
function bothValuesAreDates(strDate1, strDate2) {
    if (strDate1 && strDate2) {
        if (isStringDate(strDate1) == true && isStringDate(strDate2) == true) {
            return true;
        }
    }
    return false;
}
function isDate1GreaterThanDate2(strDate1, strDate2) {
    var compare = compareDates(strDate1, strDate2);
    if (compare != null) {
        if (compare == 1) {
            return true;
        }
    }
    return false;
}
function isDate1LessThanDate2(strDate1, strDate2) {
    var compare = compareDates(strDate1, strDate2);
    if (compare != null) {
        if (compare == -1) {
            return true;
        }
    }
    return false;
}
function isDate1EqualToDate2(strDate1, strDate2) {
    var compare = compareDates(strDate1, strDate2);
    if (compare != null) { //9/14/2015 note: code doesn't jump in here when 0 (null and 0 must be the same in js)... need to check for <> null
        if (compare == 0) {
            return true;
        }
    }
    return false;
}
function isDate1GreaterThanOrEqualToDate2(strDate1, strDate2) {
    var compare = compareDates(strDate1, strDate2);
    if (compare != null) {
        if (compare == 1 || compare == 0) {
            return true;
        }
    }
    return false;
}
function isDate1LessThanOrEqualToDate2(strDate1, strDate2) {
    var compare = compareDates(strDate1, strDate2);
    if (compare != null) {
        if (compare == -1 || compare == 0) {
            return true;
        }
    }
    return false;
}
function isDateBetweenMinimumAndMaximumDates(strDate, minDate, maxDate, datesCanBeEqual) {
    if (typeof (datesCanBeEqual) == 'undefined') {
        datesCanBeEqual = true;
    }
    var minCompare;
    var maxCompare;
    if (datesCanBeEqual == true) {
        minCompare = isDate1GreaterThanOrEqualToDate2(strDate, minDate);
        maxCompare = isDate1LessThanOrEqualToDate2(strDate, maxDate);
    } else {
        minCompare = isDate1GreaterThanDate2(strDate, minDate);
        maxCompare = isDate1LessThanDate2(strDate, maxDate);
    }
    if (minCompare == true && maxCompare == true) {
        return true;
    }
    return false;
}
function compareDates(strDate1, strDate2) {
    if (bothValuesAreDates(strDate1, strDate2) == true) {
        var date1 = new Date(strDate1);
        var date2 = new Date(strDate2);
        //return dates.compare(date1, date2); //not valid
        //return 0 for same; 1 for 1st date greater than 2nd; 2 for 1st date less than 2nd
        varDt1Day = date1.getDate(); //day of month
        varDt2Day = date2.getDate(); //day of month
        varDt1Month = date1.getMonth();
        varDt2Month = date2.getMonth();
        //varDt1Year = date1.getYear(); //appears to just be 3 digits... 2015 showed 115 so it must be years since 1900
        //varDt2Year = date2.getYear(); //appears to just be 3 digits... 2015 showed 115 so it must be years since 1900
        varDt1Year = date1.getFullYear(); //updated 9/17/2015; correctly shows 4-digit year
        varDt2Year = date2.getFullYear(); //updated 9/17/2015; correctly shows 4-digit year
        if (varDt1Year == varDt2Year) {
            //same year
            if (varDt1Month == varDt2Month) {
                //same month
                if (varDt1Day == varDt2Day) {
                    //same day
                    return 0;
                } else {
                    //different days
                    if (varDt1Day > varDt2Day) {
                        //1st day greater than 2nd
                        return 1;
                    } else {
                        //1st day less than 2nd
                        return -1;
                    }
                }
            } else {
                //different months
                if (varDt1Month > varDt2Month) {
                    //1st month greater than 2nd
                    return 1;
                } else {
                    //1st month less than 2nd
                    return -1;
                }
            }
        } else {
            //different years
            if (varDt1Year > varDt2Year) {
                //1st year greater than 2nd
                return 1;
            } else {
                //1st year less than 2nd
                return -1;
            }
        }
    }
    return null;
}
function compareDateFields(dateField1, dateField2) {
    if (dateField1 && dateField2) {
        return compareDates(dateField1.value, datefield2.value);
    }
    return null;
}
//added 9/17/2015 for testing
//function TestJQueryForId(id) {
//    var msg = "";

//    var effectiveDateField = $(id);
//    var effectiveDate = $(id).val();

//    var dt = dateValueForJQueryFieldSelector(id);
//    if (dt) {

//    }

//    return msg;
//}
//more testing 9/18/2015
//function EffectiveDateValidationMessageForAppRate() {
//    var msg = "";
//    var newDt = "";
//    var setToNewDt = false;
//    var isReset = false;

//    //var effectiveDateField = $("#txtEffectiveDate_Copy");
//    //var effectiveDate = $("#txtEffectiveDate_Copy").val();
//    //TestJQueryForId("#txtEffectiveDate_Copy");
//    var effDt = dateValueForJQueryFieldSelector("#txtEffectiveDate_Copy");
//    if (effDt) {
//        var minEffDate = dateValueForJQueryFieldSelector("#hdnAppMinimumEffectiveDate");
//        var maxEffDate = dateValueForJQueryFieldSelector("#hdnAppMaximumEffectiveDate");
//        if (minEffDate && maxEffDate) {
//            if (isDateBetweenMinimumAndMaximumDates(effDt, minEffDate, maxEffDate) == true) {
//                //valid date; everything okay
//            } else {
//                //quote date is not between minDate and maxDate
//                msg = "effective date must be between " + minEffDate + " and " + maxEffDate;
//                if (isDate1LessThanDate2(effDt, minEffDate) == true) {
//                    var updateToMinDate = false;
//                    var minQuoteDateIsGreaterThanAllDate = boolValueForJQueryFieldSelector("#hdnAppMinimumQuoteEffectiveDateIsGreaterThanDateForAllQuotes");
//                    var minEffDtAllQuotes = dateValueForJQueryFieldSelector("#hdnAppMinimumEffectiveDateAllQuotes");
//                    var quoteHasMinimunEffDt = boolValueForJQueryFieldSelector("#hdnAppQuoteHasMinimumEffectiveDate");
//                    if (minQuoteDateIsGreaterThanAllDate == true) {
//                        updateToMinDate = true;
//                    } else if (minEffDate == minEffDtAllQuotes && quoteHasMinimunEffDt == true) {
//                        updateToMinDate = true;
//                    } else {
//                        updateToMinDate = false; //redundant
//                    }
//                    if (updateToMinDate == true) {
//                        var originalEffDate = dateValueForJQueryFieldSelector("#hdnAppOriginalEffectiveDate");
//                        newDt = minEffDate;
//                        setToNewDt = true;
//                        if (isDate1EqualToDate2(originalEffDate, minEffDate) == true) {
//                            msg = "Contact Underwriting for quotes before " + minEffDate + "; date reverted back to " + minEffDate + ".";
//                            isReset = true;
//                        } else {
//                            msg = "Contact Underwriting for quotes before " + minEffDate + "; date set to " + minEffDate + ".";
//                        }
//                    } else {
//                        //quote date is less than minDate
//                        //can customize message but already defaulted above
//                    }
//                } else if (isDate1GreaterThanDate2(effDt, maxEffDate) == true) {
//                    //quote date is more than maxDate
//                    //can customize message but already defaulted above
//                } else {
//                    //shouldn't get here
//                    //can customize message but already defaulted above
//                }
//            }
//        }
//    } else {
//        msg = "invalid effective date";
//    }

//    if (msg) {
//        if (msg.length > 0) {
//            alert(msg);
//            if (newDt && setToNewDt) {
//                if (newDt.length > 0 && setToNewDt == true) {
//                    //set calendar date to newDt

//                    if (isReset == true) {
//                        //date reverted back to previous date; just a note since msg already reflects this
//                    }
//                }
//            }
//        }
//    }

//    return msg;
//}

//added 3/5/2014
function GetChildFieldOld(el, childFieldId) {
    if (el && childFieldId) {
        if (childFieldId.length > 0) {
            var children = el.childNodes;
            if (children) {
                var keepGoing = true;
                for (i = 0; i < children.length; i++) {
                    var currChild = children[i];
                    if (currChild) {
                        switch (currChild.nodeType) {
                            case 3:
                                //text Node
                                break;
                            case 1:
                                //Element node
                                if (currChild.id.length > 0) {
                                    //alert('currChild.id = ' + currChild.id + '; childFieldId = ' + childFieldId);
                                    if (currChild.id.indexOf(childFieldId) != -1) {
                                        //alert('match!');
                                        return currChild;
                                        keepGoing = false;
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                        if (!keepGoing) break; //exit for
                    }
                }
            }
        }
    }
    return null;
}
//added 1/7/2015 for testing... ended up adding localName param and using this one instead of old function (old one re-named)
function GetChildField(el, childFieldId, localName) {
    if (el && childFieldId) {
        if (childFieldId.length > 0) {
            var children = el.childNodes;
            if (children) {
                var keepGoing = true;
                for (i = 0; i < children.length; i++) {
                    var currChild = children[i];
                    if (currChild) {
                        if (typeof (localName) == 'undefined') { //new 1/7/2015
                            localName = '';
                        }
                        switch (currChild.nodeType) {
                            case 3:
                                //text Node
                                break;
                            case 1:
                                //Element node
                                if (currChild.id.length > 0) {
                                    //alert('currChild.id = ' + currChild.id + '; childFieldId = ' + childFieldId);
                                    if (currChild.id.indexOf(childFieldId) != -1) {
                                        if (localName.length > 0) { //new 1/7/2015; previous logic in ELSE
                                            if (currChild.localName) {
                                                if (currChild.localName.length > 0) {
                                                    if (currChild.localName.toUpperCase() == localName.toUpperCase()) {
                                                        //alert('match!');
                                                        return currChild;
                                                        keepGoing = false;
                                                    }
                                                }
                                            }
                                        } else {
                                            //alert('match!');
                                            return currChild;
                                            keepGoing = false;
                                        }
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                        if (!keepGoing) break; //exit for
                    }
                }
            }
        }
    }
    return null;
}
function RetainEditAndViewSectionDisplaysForSpans() {
    var spans = document.getElementsByTagName('span');
    if (spans) {
        for (var i = 0; i < spans.length; i++) {
            if (spans[i].id.length > 0) {
                if (spans[i].id.indexOf('ViewSection') != -1 || spans[i].id.indexOf('EditSection') != -1) {
                    var hdnSect_DisplayId;
                    if (spans[i].id.indexOf('ViewSection') != -1) {
                        hdnSect_DisplayId = 'ViewSect_Display';
                    } else {
                        hdnSect_DisplayId = 'EditSect_Display';
                    }
                    var hdnSect_Display = GetRelatedChildField(spans[i], hdnSect_DisplayId);
                    if (hdnSect_Display) {
                        if (hdnSect_Display.value == 'inline') {
                            //inline... show
                            spans[i].style.display = "inline";
                        } else {
                            //none or not set... hide
                            spans[i].style.display = "none";
                        }
                    }
                }
            }
        }
    }
}
//added 3/5/2014 for testing... works (added OnClientAfterSelectionChanged="TestBDP" to BasicDatePicker)
function TestBDP(sender) {
    var selectedDate = sender.getSelectedDate();//BDP js function
    //var selectedDate = getSelectedDate(sender.id);//BDP js function
    alert('selected date: ' + selectedDate); //showed Wed Feb 26 00:00:00 EST 2014
    sender.setSelectedDate(sender.getSelectedDate().addDays(7))//causes endless loop since OnClientAfterSelectionChanged keeps getting fired
    var selectedDate2 = sender.getSelectedDate();//BDP js function
    alert('new selected date: ' + selectedDate2); //showed Wed Mar 5 00:00:00 EST 2014
}
//added 3/5/2014 for testing... to see if I can detect what was clicked to cause the onblur... need to ignore if going from BDP textbox to calendar
function clickHandler(e) {
    var elem, evt = e ? e : event;
    if (evt.srcElement) elem = evt.srcElement;
    else if (evt.target) elem = evt.target;

    if (elem.id.length > 0) {
        alert(''
     + 'You clicked the following HTML element id: \n <'
     + elem.id.toUpperCase()
     + '>'
    )
    } else {
        alert(''
     + 'You clicked the following HTML element: \n <'
     + elem.tagName.toUpperCase()
     + '>'
    )
    }
    return true;
}

//document.onclick = clickHandler;

//added 3/6/2014 for testing... uses BDP js
//function BasicDatePickerSelectionChanged(sender) {
function BasicDatePickerCalendarClosed(sender) {
    //alert('BasicDatePickerCalendarClosed function');
    if (sender) {
        //alert('sender id: ' + sender.id);
        //alert('sender id: ' + sender.getControlId());
        //alert('sender id: ' + sender.clientID);
        //get dom element for sender... sender is BDP object used w/ BDP js api
        var bdp = document.getElementById(sender.clientID);
        if (bdp) {
            //var hdnIsVisible_BdpEffectiveDateCalendar = GetRelatedChildField(sender, 'hdnIsVisible_BdpEffectiveDateCalendar');//doesn't work w/ sender
            var hdnIsVisible_BdpEffectiveDateCalendar = GetRelatedChildField(bdp, 'hdnIsVisible_BdpEffectiveDateCalendar');
            if (hdnIsVisible_BdpEffectiveDateCalendar) {
                hdnIsVisible_BdpEffectiveDateCalendar.value = 'no';
            }
        }
        if (sender.input) {
            if (document.activeElement) {
                if (document.activeElement == sender.input) {
                    //keep calendar open if focus is in textbox
                    //if (sender.calendar) {
                    //    sender.calendar.style.visibility = 'visible';//doesn't work
                    //}
                    //if (sender.button) {
                    //    sender.button.click();//doesn't work
                    //}
                    //sender.input.click(); //try to get the calendar to open back up; doesn't work
                    //if (sender.calendar) {
                    //    sender.calendar.click(); //.open isn't valid and .click doesn't work
                    //}
                    //return false;//doesn't stop calendar from closing
                    return;
                }
            }
            //alert('bout ready to execute onblur for calendar close');
            OnBlurQuoteDescriptionOrEffDate(sender.input);
        }
    }
}
function BasicDatePickerCalendarOpened(sender) {
    if (sender) {
        //alert('sender id: ' + sender.getControlId());
        //get dom element for sender... sender is BDP object used w/ BDP js api
        var bdp = document.getElementById(sender.clientID);
        if (bdp) {
            //var hdnIsVisible_BdpEffectiveDateCalendar = GetRelatedChildField(sender, 'hdnIsVisible_BdpEffectiveDateCalendar');//doesn't work w/ sender
            var hdnIsVisible_BdpEffectiveDateCalendar = GetRelatedChildField(bdp, 'hdnIsVisible_BdpEffectiveDateCalendar');
            if (hdnIsVisible_BdpEffectiveDateCalendar) {
                hdnIsVisible_BdpEffectiveDateCalendar.value = 'yes';
            }
        }
    }
}
function BasicDatePickerTextboxOnblur(sender) {//for BDP textbox; only works on calendar click because BasicDatePickerCalendarClosed still executes... doesn't work as well whenever user is clicking through months since edit section goes away and calendar is left open by itself... but it still works
    //alert('BasicDatePickerTextboxOnblur function');
    //alert('sender id: ' + sender.id);
    if (sender) {
        var parent = GetParentElementForChild(sender);
        if (parent) {
            //alert('parent id: ' + parent.id);
            //alert('parent id: ' + parent.getControlId());
            //alert('parent id: ' + parent.clientID);
            //if (parent.calendar) {
            //    //alert('parent has calendar');//never getting here
            //    if (parent.calendar.style.visibility == 'visible') {
            //        //alert('calendar is open... ignore');//never getting here
            //        return;
            //    }
            //}
            //var bdpImage = GetChildField(parent, 'bdpEffectiveDate_Image');
            //if (bdpImage) {
            //    return;//never gets here
            //}
            var hdnIsVisible_BdpEffectiveDateCalendar = GetRelatedChildField(parent, 'hdnIsVisible_BdpEffectiveDateCalendar');
            if (hdnIsVisible_BdpEffectiveDateCalendar) {
                if (hdnIsVisible_BdpEffectiveDateCalendar.value == 'yes') {
                    return;
                }
            }
            //OnBlurQuoteDescriptionOrEffDate(sender);
        }
        //alert('bout ready to execute onblur for textbox');
        OnBlurQuoteDescriptionOrEffDate(sender);
    }
}
function SimulateDescOrEffDateEditButtonFromHeader(header) {
    if (InEditMode(true) == true) {
        return;
    }

    if (header) {
        var viewSection = GetRelatedChildField(header, 'ViewSection');
        SimulateDescOrEffDateEditButtonFromViewSection(viewSection, header.id); //updated to use new function 3/7/2014
    }
}
function SimulateDescOrEffDateEditButtonFromLabel(lbl) {
    //alert('SimulateDescOrEffDateEditButtonFromLabel...');
    if (InEditMode(true) == true) {
        return;
    }
    //alert('SimulateDescOrEffDateEditButtonFromLabel... continuing');

    if (lbl) {
        var viewSection = GetParentElementForChild(lbl);
        SimulateDescOrEffDateEditButtonFromViewSection(viewSection, lbl.id); //updated to use new function 3/7/2014
    }
}
//added 3/7/2014 to consolidate logic
function SimulateDescOrEffDateEditButtonFromViewSection(viewSection, fieldId) {
    if (viewSection) {
        var editSection = GetRelatedChildField(viewSection, 'EditSection');
        if (editSection) {
            viewSection.style.display = "none"; //hide ViewSection
            var hdnViewSect_Display = GetRelatedChildField(viewSection, 'ViewSect_Display');
            if (hdnViewSect_Display) {
                hdnViewSect_Display.value = 'none'; //need to retain for postbacks
            }
            editSection.style.display = "inline"; //show EditSection
            var hdnEditSect_Display = GetRelatedChildField(viewSection, 'EditSect_Display');
            if (hdnEditSect_Display) {
                hdnEditSect_Display.value = 'inline'; //need to retain for postbacks
            }
            if (fieldId.length > 0) {
                var hdnId;
                var txtId;
                var txtParent = editSection;
                if (fieldId.indexOf('QuoteDescription') != -1) {
                    hdnId = 'hdnOriginalQuoteDescription';
                    txtId = 'txtQuoteDescription';
                } else if (fieldId.indexOf('EffectiveDate') != -1) {
                    hdnId = 'hdnOriginalEffectiveDate';
                    txtId = 'TextBox'; //cphMain_ctlTreeView_bdpEffectiveDate_TextBox... didn't find match on textbox because of casing
                    txtParent = GetChildField(editSection, 'bdpEffectiveDate');
                }
                if (txtParent) {
                    if (hdnId.length > 0 && txtId.length > 0) {
                        var txt = GetChildField(txtParent, txtId);
                        if (txt) {
                            var hdn = GetRelatedChildField(viewSection, hdnId);
                            if (hdn) {
                                if (hdn.value.length > 0) {
                                    var isOkay = false;
                                    if (hdnId.indexOf('EffectiveDate') != -1) {
                                        var testDt = new Date(hdn.value);
                                        isOkay = isDate(testDt);
                                    } else {
                                        isOkay = true;
                                    }
                                    if (isOkay == true) {
                                        txt.value = hdn.value;
                                    }
                                }
                            }
                            txt.focus();
                        }
                    }
                }
            }
            SelectParentListItem(viewSection);
            //ToggleEditMode(true);
            //updated to new method so it wouldn't do overlay
            SwitchEditMode(true);
        }
    }
}

//added 3/7/2014
//function GetDateFormatForDatePickerElement(elBDP) {
//    if (elBDP) {
//        if (elBDP.innerText.length > 0) {
//            var dateFormatIndex = elBDP.innerText.indexOf('dateFormat:"');
//            if (dateFormatIndex != -1) {
//                //var strDateFormat = elBDP.innerText.substr(dateFormatIndex, elBDP.innerText.length - dateFormatIndex - 12); //length of 'dateFormat:"' = 12... can't subtract both in one step
//                var strDateFormat = elBDP.innerText.substr(dateFormatIndex, elBDP.innerText.length - dateFormatIndex);
//                strDateFormat = strDateFormat.substr(12, strDateFormat.length - 12);
//                //alert(strDateFormat);
//                if (strDateFormat.length > 0) {
//                    var closingQuoteIndex = strDateFormat.indexOf('"');
//                    if (closingQuoteIndex != -1) {
//                        strDateFormat = strDateFormat.substr(0, closingQuoteIndex);
//                        //alert(strDateFormat);
//                        if (strDateFormat.length > 0) {
//                            return strDateFormat;
//                        }
//                    }
//                }
//            }
//        }
//    }
//    return '';
//}
//rewrote 6/11/2014 since innerText doesn't work w/ FireFox
function GetDateFormatForDatePickerElement(elBDP) {
    if (elBDP) {
        var bdpInnerText = '';
        if (elBDP.innerText) {
            bdpInnerText = elBDP.innerText;
        } else if (elBDP.textContent) {
            bdpInnerText = elBDP.textContent;
        } else if (elBDP.innerHTML) {
            bdpInnerText = elBDP.innerHTML;
        }
        if (bdpInnerText.length > 0) {
            var dateFormatIndex = bdpInnerText.indexOf('dateFormat:"');
            if (dateFormatIndex != -1) {
                //var strDateFormat = bdpInnerText.substr(dateFormatIndex, bdpInnerText.length - dateFormatIndex - 12); //length of 'dateFormat:"' = 12... can't subtract both in one step
                var strDateFormat = bdpInnerText.substr(dateFormatIndex, bdpInnerText.length - dateFormatIndex);
                strDateFormat = strDateFormat.substr(12, strDateFormat.length - 12);
                //alert(strDateFormat);
                if (strDateFormat.length > 0) {
                    var closingQuoteIndex = strDateFormat.indexOf('"');
                    if (closingQuoteIndex != -1) {
                        strDateFormat = strDateFormat.substr(0, closingQuoteIndex);
                        //alert(strDateFormat);
                        if (strDateFormat.length > 0) {
                            return strDateFormat;
                        }
                    }
                }
            }
        }
    }
    return '';
}
//added 3/10/2014 for testing; 3/17/2014 note: not being used
function BasicDatePickerCalendarOnblur(sender) {//for BDP calendar
    alert('BasicDatePickerCalendarOnblur function');
    //alert('sender id: ' + sender.id);
    if (sender) {
        var parent = GetParentElementForChild(sender); //BDP
        if (parent) {
            var BdpTextbox = GetChildField(parent, 'TextBox');
            if (BdpTextbox) {
                if (document.activeElement) {
                    if (document.activeElement == BdpTextbox) {
                        alert('BasicDatePickerCalendarOnblur... BDP textbox has focus');
                        return;
                    }
                }
            }
        }
        //alert('bout ready to execute onblur for calendar');
        //OnBlurQuoteDescriptionOrEffDate(sender);
    }
}
//added 3/10/2014 for testing... currently called w/ ClickRelatedChildButton and w/ quote description and effective date label clicks
var clickedElement = null;
var suppressedEditModeMessageOnClick = 'no';
var clickedElementSourceType = '';
function SetClickedElement(el, sourceType) {
    if (el) {
        clickedElement = el;
        //if (clickedElement.id.length > 0) {
        //    alert('clicked element id: ' + clickedElement.id);
        //}
        if (typeof (sourceType) == 'undefined') {
            sourceType = 'normal';
        }
        clickedElementSourceType = sourceType;
    }
}
//added 3/11/2014
var clickedElementRelatedButtonId = '';
function SetClickedElementWithRelatedChildButtonId(el, relatedButtonId) {
    if (el) {
        clickedElementRelatedButtonId = relatedButtonId;
        SetClickedElement(el);
    }
}
//added 3/10/2014
function QuoteDescriptionOrEffectiveDateHeaderClick(header) {
    //added 5/15/2014
    //var parent = GetParentElementForChild(header); //section header
    //if (parent) {
    //    if (IsTextInWord(parent.className, 'disabledSectionHeader') == true) {
    //        //alert('You cannot access this feature right now.');
    //        return;
    //    }
    //}
    if (HasAnyDisabledParentListItem(header) == true) {
        return;
    }

    //alert('QuoteDescriptionOrEffectiveDateHeaderClick function...') //onblur happens before this function so InEditMode is already false... only works as intended when calendar is still open
    SetClickedElement(header, 'header');
    var showError = false;
    //if (header.className == 'clickable') { //3/12/2014 note: may need to check for clickableHeader instead
    //updated 3/17/2014 to just look for clickable and ignore if not
    var okayToContinue = true; //added 3/17/2014
    if (header.className.indexOf('clickable') != -1) {
        showError = true;
        //alert('okayToContinue');
    } else { //added ELSE 3/17/2014
        okayToContinue = false;
        //alert('not okayToContinue');
    }
    if (okayToContinue == true) { //added IF 3/17/2014; was previously happening every time
        if (InEditMode(showError) == false) {
            ToggleTitleAndClickableClass(header, false);
            SimulateDescOrEffDateEditButtonFromHeader(header);
        }
    }
}
function QuoteDescriptionOrEffectiveDateLabelClick(label) {
    //added 5/15/2014
    //var parent = GetParentElementForChild(label); //view section
    //if (parent) {
    //    var parent2 = GetParentElementForChild(parent); //section header
    //    if (parent2) {
    //        if (IsTextInWord(parent2.className, 'disabledSectionHeader') == true) {
    //            //alert('You cannot access this feature right now.');
    //            return;
    //        }
    //    }
    //}
    if (HasAnyDisabledParentListItem(label) == true) {
        return;
    }

    SetClickedElement(label, 'label');
    if (InEditMode(true) == false) {
        ToggleTitleAndClickableClassForRelatedParentSibling(label, false);
        SimulateDescOrEffDateEditButtonFromLabel(label);
    }
}
//added 3/11/2014
function FormatDateString(strDate, dateFormat) {
    if (strDate.length > 0) {
        if (dateFormat.length > 0) {
            dateFormat = dateFormat.toUpperCase(); //in case we need to check for M, D, or Y w/o worrying about casing... assuming month is always first and then year
            if (dateFormat.indexOf("/") != -1) {
                var arDateFormat = dateFormat.split("/");
                if (arDateFormat.length == 3) {
                    var monthFormat = arDateFormat[0];
                    var dayFormat = arDateFormat[1];
                    var yearFormat = arDateFormat[2];
                    strDate.replace("-", "/");
                    if (strDate.indexOf("/") != -1) {
                        var arDate = strDate.split("/");
                        if (arDate.length == 3) {
                            var dtMonth = arDate[0];
                            var dtDay = arDate[1];
                            var dtYear = arDate[2];
                            if (monthFormat.length == 2) {
                                if (dtMonth.length == 1) {
                                    dtMonth = '0' + dtMonth;
                                }
                            } else if (monthFormat.length == 1) {
                                if (dtMonth.length == 2) {
                                    if (dtMonth.substr(0, 1) == '0') {
                                        dtMonth = dtMonth.substr(1, 1);
                                    }
                                }
                            }
                            if (dayFormat.length == 2) {
                                if (dtDay.length == 1) {
                                    dtDay = '0' + dtDay;
                                }
                            } else if (dayFormat.length == 1) {
                                if (dtDay.length == 2) {
                                    if (dtDay.substr(0, 1) == '0') {
                                        dtDay = dtDay.substr(1, 1);
                                    }
                                }
                            }
                            if (yearFormat.length == 4) {
                                if (dtYear.length == 2) {
                                    dtYear = '20' + dtYear;
                                }
                            } else if (yearFormat.length == 2) {
                                if (dtYear.length == 4) {
                                    dtYear = dtYear.substr(2, 2);
                                }
                            }
                            strDate = dtMonth + '/' + dtDay + '/' + dtYear;
                        }
                    }
                }
            } else if (dateFormat.indexOf("-") != -1) {
                var arDateFormat = dateFormat.split("-");
                if (arDateFormat.length == 3) {
                    var monthFormat = arDateFormat[0];
                    var dayFormat = arDateFormat[1];
                    var yearFormat = arDateFormat[2];
                    strDate.replace("/", "-");
                    if (strDate.indexOf("-") != -1) {
                        var arDate = strDate.split("-");
                        if (arDate.length == 3) {
                            var dtMonth = arDate[0];
                            var dtDay = arDate[1];
                            var dtYear = arDate[2];
                            if (monthFormat.length == 2) {
                                if (dtMonth.length == 1) {
                                    dtMonth = '0' + dtMonth;
                                }
                            } else if (monthFormat.length == 1) {
                                if (dtMonth.length == 2) {
                                    if (dtMonth.substr(0, 1) == '0') {
                                        dtMonth = dtMonth.substr(1, 1);
                                    }
                                }
                            }
                            if (dayFormat.length == 2) {
                                if (dtDay.length == 1) {
                                    dtDay = '0' + dtDay;
                                }
                            } else if (dayFormat.length == 1) {
                                if (dtDay.length == 2) {
                                    if (dtDay.substr(0, 1) == '0') {
                                        dtDay = dtDay.substr(1, 1);
                                    }
                                }
                            }
                            if (yearFormat.length == 4) {
                                if (dtYear.length == 2) {
                                    dtYear = '20' + dtYear;
                                }
                            } else if (yearFormat.length == 2) {
                                if (dtYear.length == 4) {
                                    dtYear = dtYear.substr(2, 2);
                                }
                            }
                            strDate = dtMonth + '-' + dtDay + '-' + dtYear;
                        }
                    }
                }
            }
        }
    }
    return strDate;
}
function CancelQuoteDescriptionOrEffectiveDateEdit(editSection, hdnOriginalField, checkDate, fieldForButtonClick) {
    var viewSection = GetRelatedChildField(editSection, 'ViewSection');
    if (viewSection) {
        //all of this should always happen, but there's a problem if view section can't be found
        viewSection.style.display = "inline"; //show ViewSection
        var hdnViewSect_Display = GetRelatedChildField(viewSection, 'ViewSect_Display');
        if (hdnViewSect_Display) {
            hdnViewSect_Display.value = 'inline'; //need to retain for postbacks
        }
        editSection.style.display = "none"; //hide EditSection
        var hdnEditSect_Display = GetRelatedChildField(viewSection, 'EditSect_Display');
        if (hdnEditSect_Display) {
            hdnEditSect_Display.value = 'none'; //need to retain for postbacks
        }
        var resetText = hdnOriginalField.value;
        var viewLabelId;
        var headerToolTip = 'Edit ';
        if (checkDate == true) {
            //effDate
            viewLabelId = 'lblEffectiveDate';
            headerToolTip = headerToolTip + 'Effective Date';
        } else {
            //quoteDesc
            //updated 6/26/2014 for UseQuoteNumberHeader logic; previous logic is in 'if (UseQuoteNumberHeader == false) {' block
            var UseQuoteNumberHeader = false;
            var hdnUseQuoteNumberHeader = GetRelatedChildField(hdnOriginalField, 'hdnUseQuoteNumberHeader');
            if (hdnUseQuoteNumberHeader) {
                if (hdnUseQuoteNumberHeader.value == 'true') {
                    UseQuoteNumberHeader = true;
                }
            }
            if (UseQuoteNumberHeader == false) {
                var hdnQuoteNumber = GetRelatedChildField(hdnOriginalField, 'hdnQuoteNumber');
                if (hdnQuoteNumber) {
                    if (hdnQuoteNumber.value.length > 0) {
                        if (resetText.length > 0) {
                            resetText = hdnQuoteNumber.value + ' - ' + resetText;
                        } else {
                            resetText = hdnQuoteNumber.value;
                        }
                    }
                }
            }
            viewLabelId = 'lblQuoteDescription';
            //headerToolTip = headerToolTip + 'Quote Description';
            //updated 7/5/2019
            var isRemarks = false;
            //var DescriptionOrRemarksText = GetNephewField(viewSection, 'QuoteDescriptionHeader', 'DescriptionOrRemarksText');
            var DescriptionOrRemarksText = GetSiblingGrandchildField(viewSection, 'QuoteDescriptionHeader', 'DescriptionOrRemarksBoldTag', 'DescriptionOrRemarksText');
            if (DescriptionOrRemarksText) {
                if (DescriptionOrRemarksText.innerHTML == 'Remarks') {
                    isRemarks = true;
                }
            }

            if (isRemarks == true) {
                headerToolTip = headerToolTip + 'Remarks';
            } else {
                headerToolTip = headerToolTip + 'Quote Description';
            }
        }
        var viewLabel = GetChildField(viewSection, viewLabelId);
        if (viewLabel) {
            viewLabel.innerHTML = resetText;
        }
        ToggleEditMode(false);
        RemoveSelectedClassFromAllListItems();
        ToggleTitleAndClickableClassForRelatedParentSibling(fieldForButtonClick, true, headerToolTip);
    }
    //if (clickedElement) {
    //    if (suppressedEditModeMessageOnClick) {
    //        if (suppressedEditModeMessageOnClick == 'yes') {
    //            suppressedEditModeMessageOnClick = 'no';

    //            if (clickedElementSourceType) {
    //                if (clickedElementSourceType == 'header') {
    //                    //if (clickedElement.className == 'clickable') { //added IF 3/12/2014... may not need (should be clickableHeader anyway)
    //                    QuoteDescriptionOrEffectiveDateHeaderClick(clickedElement);
    //                    //}
    //                } else if (clickedElementSourceType == 'label') {
    //                    QuoteDescriptionOrEffectiveDateLabelClick(clickedElement);
    //                } else if (clickedElementRelatedButtonId) { //added ELSE IF 3/11/2014
    //                    if (clickedElementRelatedButtonId.length > 0) {
    //                        ClickRelatedChildButton(clickedElement, clickedElementRelatedButtonId);
    //                    } else {
    //                        clickedElement.click();
    //                    }
    //                } else {
    //                    clickedElement.click();
    //                }
    //                clickedElement = null;
    //                clickedElementSourceType = '';
    //                clickedElementRelatedButtonId = '';
    //                return;
    //            }
    //            //alert('this should not be executing');
    //            clickedElement.click();
    //            clickedElement = null;
    //        }
    //    }
    //}
    //updated 1/7/2015 to use new function
    CheckForClickedElementAfterEditCancel();
}
//added 1/7/2015 to put the logic in 1 spot so it can be called from multiple places
function CheckForClickedElementAfterEditCancel() {
    if (clickedElement) {
        if (suppressedEditModeMessageOnClick) {
            if (suppressedEditModeMessageOnClick == 'yes') {
                suppressedEditModeMessageOnClick = 'no';

                if (clickedElementSourceType) {
                    if (clickedElementSourceType == 'header') {
                        //if (clickedElement.className == 'clickable') { //added IF 3/12/2014... may not need (should be clickableHeader anyway)
                        QuoteDescriptionOrEffectiveDateHeaderClick(clickedElement);
                        //}
                    } else if (clickedElementSourceType == 'label') {
                        QuoteDescriptionOrEffectiveDateLabelClick(clickedElement);
                    } else if (clickedElementSourceType == 'liFormType') { //added IF 1/7/2014
                        FormTypeClick(clickedElement);
                    } else if (clickedElementRelatedButtonId) { //added ELSE IF 3/11/2014
                        if (clickedElementRelatedButtonId.length > 0) {
                            ClickRelatedChildButton(clickedElement, clickedElementRelatedButtonId);
                        } else {
                            clickedElement.click();
                        }
                    } else {
                        clickedElement.click();
                    }
                    clickedElement = null;
                    clickedElementSourceType = '';
                    clickedElementRelatedButtonId = '';
                    return;
                }
                //alert('this should not be executing');
                clickedElement.click();
                clickedElement = null;
            }
        }
    }
}

//added 3/19/2014 for testing
//function sectionHeaderClick(el, sectionText) {
//    if (el && sectionText) {
//        if (sectionText.length > 0) {
//            if (event.srcElement) {
//                if (el == event.srcElement) {
//                    //the section was clicked by itself w/o something on top of it being clicked

//                    //added 5/15/2014
//                    //if (IsTextInWord(el.className, 'disabledSectionHeader') == true) {
//                    //    //alert('You cannot access this feature right now.');
//                    //    return;
//                    //}
//                    if (HasAnyDisabledParentListItem(el) == true) {
//                        return;
//                    }

//                    sectionText = sectionText.toUpperCase();
//                    //checking sectionText so we can perform different actions depending on the section
//                    //can check for quoteDesc/effDate separately and then handle everything else with ELSE or check for everything specifically
//                    if (sectionText == "QUOTEDESC") {
//                        //just label/span clicks that open edit mode
//                        //simulateQuoteDescriptionOrEffectiveDateEditClickFromSectionHeader(el);
//                    } else if (sectionText == "EFFDATE") {
//                        //just label/span clicks that open edit mode
//                        //simulateQuoteDescriptionOrEffectiveDateEditClickFromSectionHeader(el);
//                    } else if (sectionText == "PH") {
//                        //possible expand/collapse and show label/span click
//                        simulateExpandCollapseButtonFromSectionHeader(el); //ommitting expandOrCollapse parameter makes it toggle from current view; can optionally provide expand or collapse
//                        //ClickChildButton(el, 'imgBtnPolicyholders');
//                    } else if (sectionText == "DRV") {
//                        //possible expand/collapse and show label/span click
//                        simulateExpandCollapseButtonFromSectionHeader(el); //ommitting expandOrCollapse parameter makes it toggle from current view; can optionally provide expand or collapse
//                        //ClickChildButton(el, 'imgBtnDrivers');
//                    } else if (sectionText == "VEH") {
//                        //possible expand/collapse and show label/span click
//                        simulateExpandCollapseButtonFromSectionHeader(el); //ommitting expandOrCollapse parameter makes it toggle from current view; can optionally provide expand or collapse
//                        //ClickChildButton(el, 'imgBtnVehicles');
//                    } else if (sectionText == "LOC") {
//                        //possible expand/collapse and show label/span click
//                        simulateExpandCollapseButtonFromSectionHeader(el); //ommitting expandOrCollapse parameter makes it toggle from current view; can optionally provide expand or collapse
//                        //ClickChildButton(el, 'imgBtnLocations');
//                    } else if (sectionText == "COV") {
//                        //possible expand/collapse and show label/span click
//                        simulateExpandCollapseButtonFromSectionHeader(el); //ommitting expandOrCollapse parameter makes it toggle from current view; can optionally provide expand or collapse
//                        //ClickChildButton(el, 'imgBtnCoverages');
//                    } else if (sectionText == "UW") {
//                        //just show label/span click
//                        //ClickChildButton(el, 'imgBtnUnderwritingQuestions');
//                    } else if (sectionText == "QS") { //updated 4/4/2014 w/ expandCollapse
//                        //possible expand/collapse and show label/span click
//                        simulateExpandCollapseButtonFromSectionHeader(el); //ommitting expandOrCollapse parameter makes it toggle from current view; can optionally provide expand or collapse
//                        //ClickChildButton(el, 'imgBtnQuoteSummary');
//                    } else if (sectionText == "DIS") {
//                        //possible expand/collapse and show label/span click
//                        simulateExpandCollapseButtonFromSectionHeader(el); //ommitting expandOrCollapse parameter makes it toggle from current view; can optionally provide expand or collapse
//                        //ClickChildButton(el, 'imgBtnDiscounts');
//                    } else if (sectionText == "SUR") {
//                        //possible expand/collapse and show label/span click
//                        simulateExpandCollapseButtonFromSectionHeader(el); //ommitting expandOrCollapse parameter makes it toggle from current view; can optionally provide expand or collapse
//                        //ClickChildButton(el, 'imgBtnSurcharges');
//                    } else if (sectionText == "APP") { //added 5/15/2014
//                        //just show label/span click
//                        //ClickChildButton(el, 'imgBtnApplication');
//                    } else if (sectionText == "AS") { //added 5/19/2014
//                        //possible expand/collapse and show label/span click
//                        simulateExpandCollapseButtonFromSectionHeader(el); //ommitting expandOrCollapse parameter makes it toggle from current view; can optionally provide expand or collapse
//                        //ClickChildButton(el, 'imgBtnApplicationSummary');
//                    } else if (sectionText == "CR") { //added 5/19/2014
//                        //possible expand/collapse and show label/span click
//                        simulateExpandCollapseButtonFromSectionHeader(el); //ommitting expandOrCollapse parameter makes it toggle from current view; can optionally provide expand or collapse
//                        //ClickChildButton(el, 'imgBtnCreditReports'); //button is commented out
//                    }
//                }
//            }
//        }
//    }
//}
//replaced 5/19/2014
function sectionHeaderClick(el, sectionText) {
    if (el && sectionText) {
        if (sectionText.length > 0) {
            var okayToContinue = false;
            sectionText = sectionText.toUpperCase();

            //if (sectionText == "CR" || sectionText == "MVR" || sectionText == "CLUE") { // no clickable header; updated 5/20/2014 to include MVR and CLUE
            //updated 6/3/2014 to include QS and AS when header clicks are disabled (SuccessfullyRatedFlag <> true)
            var isNonClickableQuoteOrAppSummary = false;
            if (sectionText == "QS" || sectionText == "AS") {
                var hdnSuccessfullyRatedFlag = GetRelatedChildField(el, 'SummarySection_SuccessfullyRatedFlag');
                if (hdnSuccessfullyRatedFlag) {
                    if (hdnSuccessfullyRatedFlag.value == 'true') {
                        //clickable header is enabled; function as normal
                    } else {
                        //clickable header is disabled
                        isNonClickableQuoteOrAppSummary = true;
                    }
                }
            }
            if (sectionText == "CR" || sectionText == "MVR" || sectionText == "CLUE" || isNonClickableQuoteOrAppSummary == true) { // no clickable header; updated 5/20/2014 to include MVR and CLUE
                if (typeof event != 'undefined' && event.srcElement) {
                    //need to ignore if srcElement is expand/collapse image... but still needs to happen if surrounding ImageArea
                    //alert('clicked: ' + event.srcElement.id + '; el: ' + el.id);
                    if (IsTextInWord(event.srcElement.id, 'SectionSubLists_') == false) {
                        okayToContinue = true;
                    } else {
                        if (IsTextInWord(event.srcElement.id, 'ImageArea') == true) {
                            okayToContinue = true;
                        }
                    }
                }
            }
            if (okayToContinue == false) {
                if (typeof event != 'undefined' && event.srcElement) {
                    if (el == event.srcElement) {
                        //the section was clicked by itself w/o something on top of it being clicked
                        okayToContinue = true;
                    }
                }
            }

            if (okayToContinue == true) {
                //added 5/15/2014
                //if (IsTextInWord(el.className, 'disabledSectionHeader') == true) {
                //    //alert('You cannot access this feature right now.');
                //    return;
                //}
                if (HasAnyDisabledParentListItem(el) == true) {
                    return;
                }

                sectionText = sectionText.toUpperCase();
                //checking sectionText so we can perform different actions depending on the section
                //can check for quoteDesc/effDate separately and then handle everything else with ELSE or check for everything specifically
                if (sectionText == "QUOTEDESC") {
                    //just label/span clicks that open edit mode
                    //simulateQuoteDescriptionOrEffectiveDateEditClickFromSectionHeader(el);
                } else if (sectionText == "EFFDATE") {
                    //just label/span clicks that open edit mode
                    //simulateQuoteDescriptionOrEffectiveDateEditClickFromSectionHeader(el);
                } else if (sectionText == "PH") {
                    //possible expand/collapse and show label/span click
                    simulateExpandCollapseButtonFromSectionHeader(el); //ommitting expandOrCollapse parameter makes it toggle from current view; can optionally provide expand or collapse
                    //ClickChildButton(el, 'imgBtnPolicyholders');
                } else if (sectionText == "DRV") {
                    //possible expand/collapse and show label/span click
                    simulateExpandCollapseButtonFromSectionHeader(el); //ommitting expandOrCollapse parameter makes it toggle from current view; can optionally provide expand or collapse
                    //ClickChildButton(el, 'imgBtnDrivers');
                } else if (sectionText == "VEH") {
                    //possible expand/collapse and show label/span click
                    simulateExpandCollapseButtonFromSectionHeader(el); //ommitting expandOrCollapse parameter makes it toggle from current view; can optionally provide expand or collapse
                    //ClickChildButton(el, 'imgBtnVehicles');
                } else if (sectionText == "LOC") {
                    //possible expand/collapse and show label/span click
                    simulateExpandCollapseButtonFromSectionHeader(el); //ommitting expandOrCollapse parameter makes it toggle from current view; can optionally provide expand or collapse
                    //ClickChildButton(el, 'imgBtnLocations');
                } else if (sectionText == "COV") {
                    //possible expand/collapse and show label/span click
                    simulateExpandCollapseButtonFromSectionHeader(el); //ommitting expandOrCollapse parameter makes it toggle from current view; can optionally provide expand or collapse
                    //ClickChildButton(el, 'imgBtnCoverages');
                } else if (sectionText == "UW") {
                    //just show label/span click
                    //ClickChildButton(el, 'imgBtnUnderwritingQuestions');
                } else if (sectionText == "QS") { //updated 4/4/2014 w/ expandCollapse
                    //possible expand/collapse and show label/span click
                    simulateExpandCollapseButtonFromSectionHeader(el); //ommitting expandOrCollapse parameter makes it toggle from current view; can optionally provide expand or collapse
                    //if (isNonClickableQuoteOrAppSummary == false) { //added IF 6/3/2014... in case this is ever used; would've previously happened every time
                    //    ClickChildButton(el, 'imgBtnQuoteSummary');
                    //}
                } else if (sectionText == "DIS") {
                    //possible expand/collapse and show label/span click
                    simulateExpandCollapseButtonFromSectionHeader(el); //ommitting expandOrCollapse parameter makes it toggle from current view; can optionally provide expand or collapse
                    //ClickChildButton(el, 'imgBtnDiscounts');
                } else if (sectionText == "SUR") {
                    //possible expand/collapse and show label/span click
                    simulateExpandCollapseButtonFromSectionHeader(el); //ommitting expandOrCollapse parameter makes it toggle from current view; can optionally provide expand or collapse
                    //ClickChildButton(el, 'imgBtnSurcharges');
                } else if (sectionText == "APP") { //added 5/15/2014
                    //just show label/span click
                    //ClickChildButton(el, 'imgBtnApplication');
                } else if (sectionText == "AS") { //added 5/19/2014
                    //possible expand/collapse and show label/span click
                    simulateExpandCollapseButtonFromSectionHeader(el); //ommitting expandOrCollapse parameter makes it toggle from current view; can optionally provide expand or collapse
                    //if (isNonClickableQuoteOrAppSummary == false) { //added IF 6/3/2014... in case this is ever used; would've previously happened every time
                    //    ClickChildButton(el, 'imgBtnApplicationSummary');
                    //}
                } else if (sectionText == "CR") { //added 5/19/2014
                    //possible expand/collapse and show label/span click
                    simulateExpandCollapseButtonFromSectionHeader(el); //ommitting expandOrCollapse parameter makes it toggle from current view; can optionally provide expand or collapse
                    //ClickChildButton(el, 'imgBtnCreditReports'); //button is commented out
                } else if (sectionText == "MVR") { //added 5/20/2014
                    //possible expand/collapse and show label/span click
                    simulateExpandCollapseButtonFromSectionHeader(el); //ommitting expandOrCollapse parameter makes it toggle from current view; can optionally provide expand or collapse
                    //ClickChildButton(el, 'imgBtnMvrReports'); //button is commented out
                } else if (sectionText == "CLUE") { //added 5/20/2014
                    //possible expand/collapse and show label/span click
                    simulateExpandCollapseButtonFromSectionHeader(el); //ommitting expandOrCollapse parameter makes it toggle from current view; can optionally provide expand or collapse
                    //ClickChildButton(el, 'imgBtnClueReports'); //button is commented out
                } else if (sectionText == "RES") { //added 6/16/2014
                    //possible expand/collapse and show label/span click
                    simulateExpandCollapseButtonFromSectionHeader(el); //ommitting expandOrCollapse parameter makes it toggle from current view; can optionally provide expand or collapse
                    //ClickChildButton(el, 'imgBtnResidence');
                } else if (sectionText == "PLC") { //added 7/27/2015 for Farm
                    //just show label/span click
                    //ClickChildButton(el, 'imgBtnPolicyLevelCoverages');
                } else if (sectionText == "FPP") { //added 7/27/2015 for Farm
                    //just show label/span click
                    //ClickChildButton(el, 'imgBtnFarmPersonalProperty');
                } else if (sectionText == "IMRV") { //added 7/27/2015 for Farm
                    //just show label/span click
                    //ClickChildButton(el, 'imgBtnInlandMarineAndRvWatercraft');
                } else if (sectionText == "IRPM") { //added 7/27/2015 for Farm
                    //just show label/span click
                    //ClickChildButton(el, 'imgBtnIRPM');
                }  else if (sectionText == "FILEUPLOAD") { //added 7/27/2015 for Farm
                //just show label/span click
                    //ClickChildButton(el, 'imgBtnFileUpload');
                } else if (sectionText == "BILLINFO") { //added 6/13/2019 for ReadOnly/Endorsements
                    //just show label/span click
                    //ClickChildButton(el, 'imgBtnBillingInformation');
                } else if (sectionText == "PRTHIST") { //added 6/13/2019 for ReadOnly/Endorsements
                    //just show label/span click
                    //ClickChildButton(el, 'imgBtnPrintHistory');
                } else if (sectionText == "POLHIST") { //added 6/13/2019 for ReadOnly/Endorsements
                    //just show label/span click
                    //ClickChildButton(el, 'imgBtnPolicyHistory');
                } else if (sectionText == "ROUTETOUW") { //added 12/19/2022
                    //just show label/span click
                    //ClickChildButton(el, 'imgBtnRouteToUW');
                    //ClickRouteButton(el, 'DIV');
                }

            }
        }
    }
}
function simulateQuoteDescriptionOrEffectiveDateEditClickFromSectionHeader(sectionHeader) {
    if (sectionHeader) {
        var viewSection = GetChildField(sectionHeader, 'ViewSection');
        if (viewSection) {
            if (isVisible(viewSection) == true) {
                //will need to be more specific than 'lbl' if other labels are placed inside viewSection
                var lbl = GetChildField(viewSection, 'lbl');
                if (lbl) {
                    QuoteDescriptionOrEffectiveDateLabelClick(lbl); //need to use this instead of lbl.click() or ClickChildButton(viewSection, 'lbl') so it will work w/ existing edit mode logic
                }
            }
        }
    }
}
function simulateExpandCollapseButtonFromSectionHeader(sectionHeader, expandOrCollapse) {
    if (typeof (expandOrCollapse) == "undefined") {
        expandOrCollapse = "WhicheverImageIsVisible";
    }

    if (expandOrCollapse == "expand" || expandOrCollapse == "collapse") {
        expandCollapseChildSpanSublists(sectionHeader, expandOrCollapse);
    } else {
        //click whichever one is visible (if collapse is visible, then section is currently expanded... execute collapse; else if expand is visible, then section is currently collapsed... execute expand)
        toggleExpandCollapseChildSpanSublistsView(sectionHeader);
    }
}
function toggleExpandCollapseChildSpanSublistsView(el) {
    if (el) {
        var subListsExpandCollapseSpan = GetChildSublistsExpandCollapseSpan(el);
        if (subListsExpandCollapseSpan) {
            toggleExpandCollapseSublistsViewForElement(subListsExpandCollapseSpan);
        }
    }
}
function toggleExpandCollapseSublistsViewForElement(el) {
    if (el) {
        if (isVisible(el) == true) {
            var done = false;
            var imgCollapse = GetChildField(el, 'SubLists_collapse');
            if (imgCollapse) {
                if (isVisible(imgCollapse) == true) {
                    //can click image or just call function to do the same
                    done = true;
                    expandCollapseSubLists(imgCollapse, 'collapse');
                }
            }
            if (done == false) {
                var imgExpand = GetChildField(el, 'SubLists_expand');
                if (imgExpand) {
                    if (isVisible(imgExpand) == true) {
                        //can click image or just call function to do the same
                        done = true;
                        expandCollapseSubLists(imgExpand, 'expand');
                    }
                }
            }
        }
    }
}
function expandCollapseChildSpanSublists(el, expandOrCollapse) {
    //this probably isn't needed here since it's also defaulted later
    if (typeof (expandOrCollapse) == 'undefined') {
        expandOrCollapse = 'expand';
    }

    if (el) {
        var subListsExpandCollapseSpan = GetChildSublistsExpandCollapseSpan(el);
        if (subListsExpandCollapseSpan) {
            expandCollapseSublistsForElement(subListsExpandCollapseSpan, expandOrCollapse, el); //updated 3/20/2014 to pass parent element
        }
    }
}
function expandCollapseSublistsForElement(el, expandOrCollapse, parent) { //logic taken from expandCollapseAllSublistsForElements
    //this probably isn't needed here since it's also defaulted later
    if (typeof (expandOrCollapse) == 'undefined') {
        expandOrCollapse = 'expand';
    }
    //updated 3/20/2014 for new optional parameter for parent
    if (typeof (parent) == 'undefined') {
        parent = null;
    }

    if (el) {
        //updated 3/20/2014 to add clickable class when expandCollapse section is visible
        var applyClickableClass = false;

        //updated 3/13/2014 to ignore if expand/collapse span is hidden (wasn't previously being found because of server visible="False")
        //note 3/13/2014: can only do this if everything has an initial value (default to expanded... collapse image is visible)
        if (isVisible(el) == true) { //added IF 3/13/2014; was previously happening every time
            applyClickableClass = true; //added 3/20/2014

            //expandCollapseSubLists(subListsExpandCollapseSpan, expandOrCollapse);
            //this needs to get 1 of the child elements 1st
            var hdnSubListsExpandedOrCollapsed = GetChildSublistsExpandedOrCollapsedHiddenField(el);
            if (hdnSubListsExpandedOrCollapsed) {
                expandCollapseSubLists(hdnSubListsExpandedOrCollapsed, expandOrCollapse);
            }
        }
        //updated 3/20/2014; 3/21/2014 note: will need to remove logic if expand/collapse functionality is removed from header-click
        if (parent) {
            if (parent != null) {
                //parent.test();
                if (parent.tagName) {
                    if (parent.tagName.toUpperCase() == 'DIV') {
                        ToggleCssClassForElement(parent, 'clickableHeader', applyClickableClass);
                    }
                }
            }
        }
    }
}
function isVisible(el) {
    if (el) {
        if (el.style) {
            if (el.style.visibility == "hidden" || el.style.display == "none") {
                return false;
            }
        }
        return true;
    }
    return false;
}
function testSectionHeaderClick(el) {
    if (el) {
        var srcElement = event.srcElement;
        var target = event.target;
        var txt = "";
        if (srcElement) {
            if (srcElement == el) {
                txt = appendText(txt, "event.srcElement = el", "; ")
            }
            if (srcElement.id) {
                if (srcElement.id.length > 0) {
                    txt = appendText(txt, "event.srcElement.id = " + srcElement.id, "; ")
                }
            }
            if (srcElement.localName) {
                if (srcElement.localName.length > 0) {
                    txt = appendText(txt, "event.srcElement.localName = " + srcElement.localName, "; ")
                }
            }
            if (srcElement.tagName) {
                if (srcElement.tagName.length > 0) {
                    txt = appendText(txt, "event.srcElement.tagName = " + srcElement.tagName, "; ")
                }
            }
        }
        if (target) {
            if (target == el) {
                txt = appendText(txt, "event.target = el", "; ")
            }
            if (target.id) {
                if (target.id.length > 0) {
                    txt = appendText(txt, "event.target.id = " + target.id, "; ")
                }
            }
            if (target.localName) {
                if (target.localName.length > 0) {
                    txt = appendText(txt, "event.target.localName = " + target.localName, "; ")
                }
            }
            if (target.tagName) {
                if (target.tagName.length > 0) {
                    txt = appendText(txt, "event.target.tagName = " + target.tagName, "; ")
                }
            }
        }
        if (txt.length > 0) {
            alert(txt);
        } else {
            alert("sectionHeaderClick");
        }
    }
}
function appendText(existingTxt, newTxt, separator) {
    var combinedTxt = "";

    if (typeof (separator) == "undefined") {
        separator = " ";
    }

    if (existingTxt) {
        if (existingTxt.length > 0) {
            combinedTxt = existingTxt;
        }
    }

    if (newTxt) {
        if (newTxt.length > 0) {
            if (combinedTxt.length > 0) {
                combinedTxt = combinedTxt + separator;
            }
            combinedTxt = combinedTxt + newTxt;
        }
    }

    return combinedTxt;
}
//added 3/20/2014
function ToggleCssClassForElement(el, className, onFlag) {
    if (el && className) {
        if (className.length > 0) {
            if (typeof (onFlag) == "undefined") {
                onFlag = true;
            }
            var cls = el.className;
            if (onFlag == true) {
                if (cls.length > 0) {
                    cls = cls + ' ';
                }
                cls = cls + className;
            } else {
                if (cls.length > 0) {
                    if (cls == className) {
                        cls = '';
                    } else {
                        cls = cls.replace(' ' + className, ''); //in case it's after another class
                        cls = cls.replace(className + ' ', ''); //in case it's before another class
                    }
                }
            }
            el.className = cls;
        }
    }
}

//added 10/2/2014; moved in w/ project 11/6/2014
function GetParentSibling(el, parentSiblingId) {//parent's sibling
    if (el && parentSiblingId) {
        if (parentSiblingId.length > 0) {
            var parent = GetParentElementForChild(el);
            if (parent) {
                return GetRelatedChildField(parent, parentSiblingId);
            }
        }
    }
    return null;
}
function GetCousinField(el, parentSiblingId, cousinId) {//child of parent's sibling
    if (el && parentSiblingId && cousinId) {
        var parentSibling = GetParentSibling(el, parentSiblingId);
        if (parentSibling) {
            return GetChildField(parentSibling, cousinId);
        }
    }
    return null;
}
function GetNephewField(el, siblingId, nephewId) {//child of sibling
    if (el && siblingId && nephewId) {
        var sibling = GetRelatedChildField(el, siblingId);
        if (sibling) {
            return GetChildField(sibling, nephewId);
        }
    }
    return null;
}
function GetGrandchildField(el, childFieldId, grandchildFieldId) {
    if (el && childFieldId && grandchildFieldId) {
        var child = GetChildField(el, childFieldId);
        if (child) {
            return GetChildField(child, grandchildFieldId);
        }
    }
    return null;
}
function GetParentSiblingGrandchildField(el, parentSiblingId, cousinId, cousinChildId) {//grandchild of parent's sibling
    if (el && parentSiblingId && cousinId && cousinChildId) {
        var parentSibling = GetParentSibling(el, parentSiblingId);
        if (parentSibling) {
            return GetGrandchildField(parentSibling, cousinId, cousinChildId);
        }
    }
    return null;
}
function GetSiblingGrandchildField(el, siblingId, nephewId, nephewChildId) {//grandchild of sibling
    if (el && siblingId && nephewId && nephewChildId) {
        var sibling = GetRelatedChildField(el, siblingId);
        if (sibling) {
            return GetGrandchildField(sibling, nephewId, nephewChildId);
        }
    }
    return null;
}
function PropertyFormTypeDropdownOnblurTest(ddl) {
    if (ddl) {
        //alert(ddl.id);
        var val = ddl.options[ddl.selectedIndex].value;
        var txt = ddl.options[ddl.selectedIndex].text;
        //alert("val: " + val + "; txt: " + txt);

        //var lblFormTypeId = GetRelatedChildField(ddl, 'lblFormTypeId'); //will prob need to use hidden field since label has Visible="false"
        //var lblFormType = GetCousinField(ddl, 'liPropertySubItem_FormType', 'lblFormType');//current parent should be liPropertySubItem_FormTypeId
        var hdnOriginalFormTypeId = GetRelatedChildField(ddl, 'hdnOriginalFormTypeId');
        var hdnOriginalFormType = GetCousinField(ddl, 'liPropertySubItem_FormType', 'hdnOriginalFormType');

        var prevVal = 'Unknown';
        var prevTxt = 'Unknown';

        //if (lblFormTypeId) {
        //    prevVal = lblFormTypeId.value;
        //}
        //if (lblFormType) {
        //    prevTxt = lblFormType.value;
        //}
        if (hdnOriginalFormTypeId) {
            prevVal = hdnOriginalFormTypeId.value;
        }
        if (hdnOriginalFormType) {
            prevTxt = hdnOriginalFormType.value;
        }
        alert("val: " + val + "; txt: " + txt + "; prevVal: " + prevVal + "; prevTxt: " + prevTxt);
    }
}
//1/7/2015 - created new function and renamed old one
function PropertyFormTypeDropdownOnblur(ddl) {
    if (ddl) {
        var val = ddl.options[ddl.selectedIndex].value;
        var txt = ddl.options[ddl.selectedIndex].text.toUpperCase();

        var hdnOriginalFormTypeId = GetRelatedChildField(ddl, 'hdnOriginalFormTypeId');
        var hdnOriginalFormType = GetCousinField(ddl, 'liPropertySubItem_FormType', 'hdnOriginalFormType');

        var prevVal = '';
        var prevTxt = '';

        if (hdnOriginalFormTypeId) {
            prevVal = hdnOriginalFormTypeId.value;
        }
        if (hdnOriginalFormType) {
            prevTxt = hdnOriginalFormType.value.toUpperCase();
        }

        if (val == prevVal) {
            //same; cancel
            CancelFormTypeEdit(ddl, hdnOriginalFormTypeId);
        } else {
            //changed; save
            if (val.length == 0) {
                //has nothing
                var okayToContinue = confirm('please enter a form type');
                if (okayToContinue == true) {
                    ddl.focus();
                } else {
                    CancelFormTypeEdit(ddl, hdnOriginalFormTypeId);
                }
                return;
            } else {
                //has something
                var confirmMsg;
                if (txt.length > 0) {
                    if (prevTxt.length > 0) {
                        confirmMsg = "Are you sure you'd like to change your form type selection from " + prevTxt + " to " + txt + "?";
                    } else {
                        confirmMsg = "Are you sure you'd like to change your form type selection to " + txt + "?";
                    }
                } else {
                    confirmMsg = 'Are you sure about your form type selection?';
                }
                var okayToContinue = confirm(confirmMsg);
                if (okayToContinue == true) {
                    aboutToSave = true; //to suppress any edit mode messages that may be encountered
                    ClickRelatedChildButton(ddl, 'imgBtnSaveFormType', false); //false tells it to ignore Edit mode
                } else {
                    CancelFormTypeEdit(ddl, hdnOriginalFormTypeId);
                }
                return;
            }
        }
    }
}
//added 1/6/2015
function SetDropdownValue(ddl, val) {
    if (ddl && val) {
        for (i = 0; i < ddl.options.length; i++) {
            if (ddl.options[i].value == val) {
                ddl.selectedIndex = i;
                return;
            }
        }
    }
}
function FormTypeClick(li) {
    if (li) {
        if (HasAnyDisabledParentListItem(li) == true) {
            return;
        }

        SetClickedElement(li, 'liFormType'); //may need to update CancelQuoteDescriptionOrEffectiveDateEdit to handle this sourceType
        if (InEditMode(true) == false) {
            //ToggleTitleAndClickableClassForRelatedParentSibling(label, false);
            //SimulateDescOrEffDateEditButtonFromLabel(label);

            var liFormTypeId = GetRelatedChildField(li, 'liPropertySubItem_FormTypeId');
            if (liFormTypeId) {
                //hide formType li and show formTypeId li
                li.style.display = "none";
                liFormTypeId.style.display = "block";

                var ddlFormType = GetChildField(liFormTypeId, 'ddlFormType');
                if (ddlFormType) {
                    var hdnOriginalFormTypeId = GetRelatedChildField(ddlFormType, 'hdnOriginalFormTypeId');
                    if (hdnOriginalFormTypeId) {
                        SetDropdownValue(ddlFormType, hdnOriginalFormTypeId.value);
                    }
                    ddlFormType.focus();
                }
                SelectParentListItem(li);
                SwitchEditMode(true);
            }
        }
    }
}
//added 1/7/2015
function CancelFormTypeEdit(ddlFormType, hdnOriginalFormTypeId) {
    if (ddlFormType) {
        if (hdnOriginalFormTypeId) {
            SetDropdownValue(ddlFormType, hdnOriginalFormTypeId.value);
        }
        var liFormTypeId = GetParentListItem(ddlFormType);
        if (liFormTypeId) {
            var liFormType = GetRelatedChildField(liFormTypeId, 'liPropertySubItem_FormType');
            if (liFormType) {
                //hide formTypeId li and show formType li
                liFormTypeId.style.display = "none";
                liFormType.style.display = "block";

                ToggleEditMode(false);
                RemoveSelectedClassFromAllListItems();
            }
        }
    }
    CheckForClickedElementAfterEditCancel();
}

//added 9/3/2015 for testing; not currently being used as-of 9/15/2015
function RemoveAutoFocus() {
    var els = document.getElementsByTagName("input");
    if (els) {
        for (var i = 0; i < els.length; i++) {
            //var autoFocusAttribute = els[i].getAttribute("autofocus");
            //if (autoFocusAttribute) {
            //    els[i].removeAttribute("autofocus");
            els[i].removeAttribute('autofocus');
            //    //els[i].blur();
            //    //els[i].setAttribute("onfocus", "this.blur()"); //only if you want no inputs to hold focus                
            //}            
        }
    }
    //document.querySelector("[" + autofocus + "=" + autofocus + "]").removeAttribute(autofocus);
    //document.querySelector('[autofocus]').removeAttribute('autofocus');
}


/// If you get a true back then continue with post back - Matt A 8-18-14
function TreePolicyholderLabelClicked(sender) {
    if (typeof ShowPolicyHolder == 'function') {
        var index = parseInt($(sender).next().text()) - 1;
        return ShowPolicyHolder(index);
    }
    return true;
}

/// If you get a true back then continue with post back - Matt A 8-18-14
function TreeDriverLabelClicked(sender) {
    if (typeof ShowDriver == 'function') {
        var index = parseInt($(sender).next().text()) - 1;
        return ShowDriver(index);
    }
    return true;
}

/// If you get a true back then continue with post back - Matt A 8-18-14
function TreeVehicleClicked(sender) {
    if (typeof ShowVehicle == 'function') {
        var index = parseInt($(sender).next().text()) - 1;
        return ShowVehicle(index);
    }
    return true;
}

//added 2/22/2019
function QuoteTransactionTypeFlag() {
    var hdnQuoteTransactionTypeFlag = document.getElementById(hdnQuoteTransactionTypeFlagId);
    if (hdnQuoteTransactionTypeFlag) {
        return hdnQuoteTransactionTypeFlag.value;
    }
    return '';
}
function StringHasSomething(str) {
    if (str) {
        if (str.length > 0) {
            return true;
        }
    }
    return false;
}
function StringContainsText(str, txtToFind, ignoreCasing) { //ignoreCasing defaulted to false below when missing
    if (StringHasSomething(str) == true && StringHasSomething(txtToFind) == true) {
        if (str.length >= txtToFind.length) {
            if (typeof (ignoreCasing) == 'undefined') {
                ignoreCasing = false;
            }
            var strHold = str;
            var txtToFindHold = txtToFind;
            if (ignoreCasing == true) {
                strHold = strHold.toUpperCase();
                txtToFindHold = txtToFindHold.toUpperCase();
            }
            if (strHold.indexOf(txtToFindHold) != -1) {
                return true;
            }
        }
    }
    return false;
}
function IsReadOnlyTransaction(tranFlag) {
    if (typeof (tranFlag) == 'undefined') {
        tranFlag = QuoteTransactionTypeFlag();
    }
    if (StringHasSomething(tranFlag) == true) {
        var strToCheck = 'ReadOnly';
        return StringContainsText(tranFlag, strToCheck, true);
    }
    return false;
}
function IsEndorsementTransaction(tranFlag) {
    if (typeof (tranFlag) == 'undefined') {
        tranFlag = QuoteTransactionTypeFlag();
    }
    if (StringHasSomething(tranFlag) == true) {
        var strToCheck = 'Endorsement';
        return StringContainsText(tranFlag, strToCheck, true);
    }
    return false;
}
function IsNewBusinessTransaction(tranFlag, trueWhenNotReadOnlyOrEndorsement) { //trueWhenNotReadOnlyOrEndorsement defaulted to true below when missing
    if (typeof (tranFlag) == 'undefined') {
        tranFlag = QuoteTransactionTypeFlag();
    }
    if (StringHasSomething(tranFlag) == true) {
        var strToCheck = 'NewBusiness';
        if (StringContainsText(tranFlag, strToCheck, true) == true) {
            return true;
        } else {
            if (typeof (trueWhenNotReadOnlyOrEndorsement) == 'undefined') {
                trueWhenNotReadOnlyOrEndorsement = true;
            }
            if (trueWhenNotReadOnlyOrEndorsement == true) {
                if (IsReadOnlyTransaction(tranFlag) == false && IsEndorsementTransaction(tranFlag) == false) {
                    return true;
                }
            }
        }
    }
    return false;
}

//added 1/4/2023 (similar to ClickRelatedChildButton)
function ClickRouteButton(child, divOrSpan, checkEditMode) {
    if (typeof (checkEditMode) == 'undefined') {
        checkEditMode = true;
    }
    if (typeof (divOrSpan) == 'undefined') {
        divOrSpan = 'SPAN';
    }

    if (HasAnyDisabledParentListItem(child) == true) {
        return;
    }

    if (checkEditMode == true) {
        if (InEditMode(true) == true) {
            //alert("This functionality is currently locked.");
            return;
        }
    }

    var hdnRouteCommOrPers;
    if (divOrSpan == 'DIV') {
        hdnRouteCommOrPers = GetRelatedChildField(child, 'hdnRouteCommOrPers');
    } else { //SPAN
        hdnRouteCommOrPers = GetParentSibling(child, 'hdnRouteCommOrPers');
    }
    if (hdnRouteCommOrPers) {
        if (hdnRouteCommOrPers.value == 'COMM') {
            //COMM
            InitEmailToUW();
        } else {
            //PERS
            $("#btnRouteToUw").click();
        }
    }
}
