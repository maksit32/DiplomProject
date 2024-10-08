//authentification
export const loginPath = "http://localhost:8443/api/auth/login";

//notifyUsers
export const notifyAdminUsersPath = "http://localhost:8443/api/notify/adm";
export const notifySubUsersPath = "http://localhost:8443/api/notify/sub";
export const notifyAllUsersPath = "http://localhost:8443/api/notify/all";

//tgUsers
export const getAllTgUsersPath = "http://localhost:8443/api/tgusers/get";
export const getAdmTgUsersPath = "http://localhost:8443/api/tgusers/get_admins";
export const getSubTgUsersPath = "http://localhost:8443/api/tgusers/get_subusers";
export const getTgUserByIdPath = "http://localhost:8443/api/tgusers/id";
export const deleteTgUserPath = "http://localhost:8443/api/tgusers/delete";
export const updateTgUserPath = "http://localhost:8443/api/tgusers/update";

//scienceEvents
export const getAllScienceEventsPath = "http://localhost:8443/api/scienceevents/get";
export const getActualScienceEventsPath = "http://localhost:8443/api/scienceevents/get_actual_scienceevents";
export const getScienceEventByIdPath = "http://localhost:8443/api/scienceevents/id";
export const updateScienceEventPath = "http://localhost:8443/api/scienceevents/update";
export const addScienceEventPath = "http://localhost:8443/api/scienceevents/add";
export const deleteScienceEventPath = "http://localhost:8443/api/scienceevents/delete";

//userCreatedEvents
export const getAllUserCreatedEventsPath = "http://localhost:8443/api/usercreatedevents/get";
export const getUserCreatedEventsByChatIdPath = "http://localhost:8443/api/usercreatedevents/id";
export const updateUserCreatedEventsPath = "http://localhost:8443/api/usercreatedevents/update";
export const deleteUserCreatedEventPath = "http://localhost:8443/api/usercreatedevents/delete";

//documents
export const getSNOFilesPath = "http://localhost:8443/api/Documents/sno";
export const getSMUFilesPath = "http://localhost:8443/api/Documents/smu";
export const downloadSNOFilePath = "http://localhost:8443/api/Documents/sno/";
export const downloadSMUFilePath = "http://localhost:8443/api/Documents/smu/";
export const deleteSNOFilePath = "http://localhost:8443/api/Documents/sno";
export const deleteSMUFilePath = "http://localhost:8443/api/Documents/smu";