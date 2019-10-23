export function formatPhone(phone) {
	const number = phone.replace(/[^0-9]/g, '');
	let formattedNumber = number;

	if (number.length === 10)
		formattedNumber = "(" + number.slice(0, 3) + ") " + number.slice(3, 6) + "-" + number.slice(6, 11);

	else if (number.length === 11)
		formattedNumber = "(" + number.slice(1, 4) + ") " + number.slice(4, 7) + "-" + number.slice(7, 12);

	else if (number.length < 10)
		formattedNumber = "";

	return formattedNumber;
}