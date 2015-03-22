function long2ip(ip) {
	//  discuss at: http://phpjs.org/functions/long2ip/
	// original by: Waldo Malqui Silva
	//   example 1: long2ip( 3221234342 );
	//   returns 1: '192.0.34.166'

	if (!isFinite(ip))
		return false;

	return [ip >>> 24, ip >>> 16 & 0xFF, ip >>> 8 & 0xFF, ip & 0xFF].join('.');
}

function dateToString(date) {
	if (date) {
		var ms = date.getMilliseconds();
		if (ms <= 9)
			ms = "00" + ms;
		else if (ms <= 99)
			ms = "0" + ms;

		var s = date.getSeconds();
		if (s <= 9) s = "0" + s;

		var m = date.getMinutes();
		if (m <= 9) m = "0" + m;

		var h = date.getHours();
		if (h <= 9) h = "0" + h;

		var d = date.getDate();

		var mo = date.getMonth() + 1;

		var y = date.getFullYear();

		return mo + "/" + d + "/" + y + " " + h + ":" + m + ":" + s + "." + ms;
	} else {
		return "";
	}
}
