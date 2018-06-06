import { ChatMessageDtoReadState, ChatMessageDtoSide, CreatePaymentDtoEditionPaymentType, CreatePaymentDtoPaymentPeriodType, CreatePaymentDtoSubscriptionPaymentGatewayType, DefaultTimezoneScope, FriendDtoState, IncomeStatisticsDateInterval, IsTenantAvailableOutputState, RegisterTenantInputSubscriptionStartType, SalesSummaryDatePeriod, UserNotificationState } from '@shared/service-proxies/service-proxies';

export class AppChatMessageReadState {
    static Unread: number = ChatMessageDtoReadState.Unread;
    static Read: number = ChatMessageDtoReadState.Read;
}

export class AppChatSide {
    static Sender: number = ChatMessageDtoSide.Sender;
    static Receiver: number = ChatMessageDtoSide.Receiver;
}

export class AppFriendshipState {
    static Accepted: number = FriendDtoState.Accepted;
    static Blocked: number = FriendDtoState.Blocked;
}


export class AppTimezoneScope {
    static Application: number = DefaultTimezoneScope.Application;
    static Tenant: number = DefaultTimezoneScope.Tenant;
    static User: number = DefaultTimezoneScope.User;
}

export class AppUserNotificationState {
    static Unread: number = UserNotificationState.Unread;
    static Read: number = UserNotificationState.Read;
}

export class AppTenantAvailabilityState {
    static Available: number = IsTenantAvailableOutputState.Available;
    static InActive: number = IsTenantAvailableOutputState.InActive;
    static NotFound: number = IsTenantAvailableOutputState.NotFound;
}

export class AppIncomeStatisticsDateInterval {
    static Daily: number = IncomeStatisticsDateInterval.Daily;
    static Weekly: number = IncomeStatisticsDateInterval.Weekly;
    static Monthly: number = IncomeStatisticsDateInterval.Monthly;
}

export class SubscriptionStartType {

    static Free: number = RegisterTenantInputSubscriptionStartType.Free;
    static Trial: number = RegisterTenantInputSubscriptionStartType.Trial;
    static Paid: number = RegisterTenantInputSubscriptionStartType.Paid;
}

export class EditionPaymentType {
    static NewRegistration: number = CreatePaymentDtoEditionPaymentType.NewRegistration;
    static BuyNow: number = CreatePaymentDtoEditionPaymentType.BuyNow;
    static Upgrade: number = CreatePaymentDtoEditionPaymentType.Upgrade;
    static Extend: number = CreatePaymentDtoEditionPaymentType.Extend;
}

export class AppEditionExpireAction {
    static DeactiveTenant = 'DeactiveTenant';
    static AssignToAnotherEdition = 'AssignToAnotherEdition';
}

export class PaymentPeriodType {
    static Monthly: number = CreatePaymentDtoPaymentPeriodType.Monthly;
    static Annual: number = CreatePaymentDtoPaymentPeriodType.Annual;
}

export class SubscriptionPaymentGatewayType {
    static Paypal: number = CreatePaymentDtoSubscriptionPaymentGatewayType.Paypal;
}

export class AppSalesSummaryDatePeriod {
    static Daily: number = SalesSummaryDatePeriod.Daily;
    static Weekly: number = SalesSummaryDatePeriod.Weekly;
    static Monthly: number = SalesSummaryDatePeriod.Monthly;
}

