export const getToken = () => localStorage.getItem("token");


export const authFetch = (url: string, options: RequestInit = {}) => {
return fetch(url, {
...options,
headers: {
...(options.headers || {}),
Authorization: `Bearer ${getToken()}`,
},
});
};