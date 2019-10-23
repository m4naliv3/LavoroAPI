export function SetMessages(payload){
    return{
        type: 'SET_MESSAGES', 
        payload: payload
    };
}
export function SetAccount(payload){
    return{
        type: 'SET_ACCOUNT', 
        payload: payload
    };
}
export function SetConversation(payload){
    return{
        type: 'SET_CONVERSATION', 
        payload: payload
    };
}
export function OpenContactModal(payload){
    return{
        type: 'OPEN_CONTACT_MODAL', 
        payload: payload
    };
}
export function SetOutboundMessage(payload){
    return{
        type: 'SET_OUTBOUND_MESSAGE', 
        payload: payload
    };
}
export function SetAccountPhone(payload){
    return{
        type: 'SET_ACCOUNT_PHONE', 
        payload: payload
    };
}
export function SetContacts(payload){
    return{
        type: 'SET_CONTACTS', 
        payload: payload
    };
}