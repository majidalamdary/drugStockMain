function CurrentMenu(menu) {
    var elem;
    
    switch (menu) {
        
    
        case 'dashboardmenu':
            $('#dashboardmenu').addClass('active');
            break;

        case 'listelambarusermenu':
            $('#listelambarusermenu').addClass('active');
            break;

            
        case 'listelambar':
            $('#listelambar').addClass('active');
            break;
            

        case 'keyvaluemenu':
            $('#keyvaluemenu').addClass('active');
            break;

        case 'weightmenu':
            $('#weightmenu').addClass('active');
            break;

        case 'commentmenu':
            $('#commentmenu').addClass('active');
            break;

        case 'listsupportmenu':
            $('#listsupportmenu').addClass('active');
            break;

        case 'listsupportanomenu':
            $('#listsupportanomenu').addClass('active');
            break;

        case 'messagemenu':
            $('#messagemenu').addClass('active');
            break;
            

        case 'listofficemenu':
            $('#listofficemenu').addClass('active');
            //$('#listofficemenu').addClass('active');
            //$('#listofficemenu').parents("li").addClass('active');
            //$('#listofficemenu').parents(".hidden-ul").css("display", "block");
            break;

        case 'listdrivermenu':
            $('#listdrivermenu').addClass('active');
            //$('#listdrivermenu').parents("li").addClass('active');
            //$('#listdrivermenu').parents(".hidden-ul").css("display", "block");
            break;

        case 'listcartypemenu':
            $('#listcartypemenu').addClass('active');
            //$('#listdrivermenu').parents("li").addClass('active');
            //$('#listdrivermenu').parents(".hidden-ul").css("display", "block");
            break;

        case 'listvehiclebrand':
            $('#listvehiclebrand').addClass('active');
            //$('#listdrivermenu').parents("li").addClass('active');
            //$('#listdrivermenu').parents(".hidden-ul").css("display", "block");
            break;

        case 'PlanReport':
            $('#PlanReportMenu').addClass('active');
            $('#PlanReportMenu').parents("li").addClass('active');
            $('#PlanReportMenu').parents(".hidden-ul").css("display", "block");
            break;

        case 'delivededPayloadsreportmenu':
            $('#delivededPayloadsreportmenu').addClass('active');
            $('#delivededPayloadsreportmenu').parents("li").addClass('active');
            $('#delivededPayloadsreportmenu').parents(".hidden-ul").css("display", "block");
            break;
        case 'ListPayloadOwnerDriver':
            $('#ListPayloadOwnerDriver').addClass('active');
            $('#ListPayloadOwnerDriver').parents("li").addClass('active');
            $('#ListPayloadOwnerDriver').parents(".hidden-ul").css("display", "block");
            break;
        case 'CreatePayloadOwnerDriver':
            $('#ListPayloadOwnerDriver').addClass('active');
            $('#ListPayloadOwnerDriver').parents("li").addClass('active');
            $('#ListPayloadOwnerDriver').parents(".hidden-ul").css("display", "block");
            break;
        case 'ListContractmenu':
            $('#ListContractmenu').addClass('active');
            $('#ListContractmenu').parents("li").addClass('active');
            $('#ListContractmenu').parents(".hidden-ul").css("display", "block");
            break;
        case 'ListBrokerDigitalBagmenu':
            $('#ListBrokerDigitalBagmenu').addClass('active');
            $('#ListBrokerDigitalBagmenu').parents("li").addClass('active');
            $('#ListBrokerDigitalBagmenu').parents(".hidden-ul").css("display", "block");
            break;
        case 'createAppUpdatemenu':
            $('#createAppUpdatemenu').addClass('active');
            $('#createAppUpdatemenu').parents("li").addClass('active');
            $('#createAppUpdatemenu').parents(".hidden-ul").css("display", "block");
            break;
        case 'listAppUpdatemenu':
            $('#listAppUpdatemenu').addClass('active');
            $('#listAppUpdatemenu').parents("li").addClass('active');
            $('#listAppUpdatemenu').parents(".hidden-ul").css("display", "block");
            break;
        case 'WithDrawalListmenu':
            $('#WithDrawalListmenu').addClass('active');
            $('#WithDrawalListmenu').parents("li").addClass('active');
            $('#WithDrawalListmenu').parents(".hidden-ul").css("display", "block");
            break;
        case 'PayloadOwnersActivityeportmenu':
            $('#PayloadOwnersActivityeportmenu').addClass('active');
            $('#PayloadOwnersActivityeportmenu').parents("li").addClass('active');
            $('#PayloadOwnersActivityeportmenu').parents(".hidden-ul").css("display", "block");
            break;
        case 'barnamehreportmenu':
            $('#barnamehreportmenu').addClass('active');
            $('#barnamehreportmenu').parents("li").addClass('active');
            $('#barnamehreportmenu').parents(".hidden-ul").css("display", "block");
            break;
        case 'paidkerayemenu':
            $('#paidkerayemenu').addClass('active');
            $('#paidkerayemenu').parents("li").addClass('active');
            $('#paidkerayemenu').parents(".hidden-ul").css("display", "block");
            break;
        case 'transactionListreportmenu':
            $('#transactionListreportmenu').addClass('active');
            $('#transactionListreportmenu').parents("li").addClass('active');
            $('#transactionListreportmenu').parents(".hidden-ul").css("display", "block");
            break;
        case 'chargeListreportmenu':
            $('#chargeListreportmenu').addClass('active');
            $('#chargeListreportmenu').parents("li").addClass('active');
            $('#chargeListreportmenu').parents(".hidden-ul").css("display", "block");
            break;
        case 'chargemenu':
            $('#chargemenu').addClass('active');
            $('#chargemenu').parents("li").addClass('active');
            $('#chargemenu').parents(".hidden-ul").css("display", "block");
            break;

        case 'ContractorPlan':
            $('#ContractorPlanMenu').addClass('active');
            $('#ContractorPlanMenu').parents("li").addClass('active');
            $('#ContractorPlanMenu').parents(".hidden-ul").css("display", "block");
            break;

        case 'createpayloadmenu':
            $('#createpayloadmenu').addClass('active');
            $('#createpayloadmenu').parents("li").addClass('active');
            $('#createpayloadmenu').parents(".hidden-ul").css("display", "block");
            break;
        case 'CreateDigitalBag':
            $('#CreateDigitalBag').addClass('active');
            $('#CreateDigitalBag').parents("li").addClass('active');
            $('#CreateDigitalBag').parents(".hidden-ul").css("display", "block");
            break;
        case 'createpayloadmenureceive':
            $('#createpayloadmenureceive').addClass('active');
            $('#createpayloadmenureceive').parents("li").addClass('active');
            $('#createpayloadmenureceive').parents(".hidden-ul").css("display", "block");
            break;
        case 'createpayloadmenu':
            $('#createpayloadmenu').addClass('active');
            $('#createpayloadmenu').parents("li").addClass('active');
            $('#createpayloadmenu').parents(".hidden-ul").css("display", "block");
            break;
        case 'ListKnownReceiver':
            $('#ListKnownReceiver').addClass('active');
            $('#ListKnownReceiver').parents("li").addClass('active');
            $('#ListKnownReceiver').parents(".hidden-ul").css("display", "block");
            break;
        case 'NewKnownReceiver':
            $('#NewKnownReceiver').addClass('active');
            $('#NewKnownReceiver').parents("li").addClass('active');
            $('#NewKnownReceiver').parents(".hidden-ul").css("display", "block");
            break;
        case 'sendnotiftodrivermenu':
            $('#sendnotiftodrivermenu').addClass('active');
            $('#sendnotiftodrivermenu').parents("li").addClass('active');
            $('#sendnotiftodrivermenu').parents(".hidden-ul").css("display", "block");
            break;

        case 'listpayloadmenu':
            $('#listpayloadmenu').addClass('active');
            $('#listpayloadmenu').parents("li").addClass('active');
            $('#listpayloadmenu').parents(".hidden-ul").css("display", "block");
            break;

        case 'listpayloadtypemenu':
            $('#listpayloadtypemenu').addClass('active');
            $('#listpayloadtypemenu').parents("li").addClass('active');
            $('#listpayloadtypemenu').parents(".hidden-ul").css("display", "block");
            break;

        case 'listusermenu':
            $('#listusermenu').addClass('active');
            $('#listusermenu').parents("li").addClass('active');
            $('#listusermenu').parents(".hidden-ul").css("display", "block");
            break;
            
        case 'listrolemenu':
            $('#listrolemenu').addClass('active');
            $('#listrolemenu').parents("li").addClass('active');
            $('#listrolemenu').parents(".hidden-ul").css("display", "block");
            break;

        case 'listtariffmenu':
            $('#listtariffmenu').addClass('active');
            $('#listtariffmenu').parents("li").addClass('active');
            $('#listtariffmenu').parents(".hidden-ul").css("display", "block");
            break;

        case 'increasetariffmenu':
            $('#increasetariffmenu').addClass('active');
            $('#increasetariffmenu').parents("li").addClass('active');
            $('#increasetariffmenu').parents(".hidden-ul").css("display", "block");
            break;
            
        case 'chargeofficemenu':
            $('#chargeofficemenu').addClass('active');
            $('#chargeofficemenu').parents("li").addClass('active');
            $('#chargeofficemenu').parents(".hidden-ul").css("display", "block");
            break;
        case 'CostPerPayloadmenu':
            $('#CostPerPayloadmenu').addClass('active');
            $('#CostPerPayloadmenu').parents("li").addClass('active');
            $('#CostPerPayloadmenu').parents(".hidden-ul").css("display", "block");
            break;
        case 'increasePayloadOwnerChargemenu':
            $('#increasePayloadOwnerChargemenu').addClass('active');
            $('#increasePayloadOwnerChargemenu').parents("li").addClass('active');
            $('#increasePayloadOwnerChargemenu').parents(".hidden-ul").css("display", "block");
            break;
        case 'increaseChargemenu':
            $('#increaseChargemenu').addClass('active');
            $('#increaseChargemenu').parents("li").addClass('active');
            $('#increaseChargemenu').parents(".hidden-ul").css("display", "block");
            break;
        case 'listOwnerpayloadmenu':
            $('#listOwnerpayloadmenu').addClass('active');
            $('#listOwnerpayloadmenu').parents("li").addClass('active');
            $('#listOwnerpayloadmenu').parents(".hidden-ul").css("display", "block");
            break;

        case 'chargedrivermenu':
            $('#chargedrivermenu').addClass('active');
            $('#chargedrivermenu').parents("li").addClass('active');
            $('#chargedrivermenu').parents(".hidden-ul").css("display", "block");
            break;

        case 'liststatemenu':
            $('#liststatemenu').addClass('active');
            $('#liststatemenu').parents("li").addClass('active');
            $('#liststatemenu').parents(".hidden-ul").css("display", "block");
            break;

        case 'listcitymenu':
            $('#listcitymenu').addClass('active');
            $('#listcitymenu').parents("li").addClass('active');
            $('#listcitymenu').parents(".hidden-ul").css("display", "block");
            break;

        case 'allpayloadsreportmenu':
            $('#allpayloadsreportmenu').addClass('active');
            $('#allpayloadsreportmenu').parents("li").addClass('active');
            $('#allpayloadsreportmenu').parents(".hidden-ul").css("display", "block");
            break;
          
        case 'generalreportmenu':
            $('#generalreportmenu').addClass('active');
            $('#generalreportmenu').parents("li").addClass('active');
            $('#generalreportmenu').parents(".hidden-ul").css("display", "block");
            break;

        case 'rssnewsmenu':
            $('#rssnewsmenu').addClass('active');
            $('#rssnewsmenu').parents("li").addClass('active');
            $('#rssnewsmenu').parents(".hidden-ul").css("display", "block");
            break;

        case 'newsmenu':
            $('#newsmenu').addClass('active');
            $('#newsmenu').parents("li").addClass('active');
            $('#newsmenu').parents(".hidden-ul").css("display", "block");
            break;
    }
}

//$(document).on("click", ".innernavigationelement", function () {
//    $('#dashboardmenu').removeClass('active');
//})