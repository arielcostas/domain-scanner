export function getRelativeTimeString(date: Date): string {
    const relativeTimeFormatter = new Intl.RelativeTimeFormat();
	const diff = Date.now() - date.getTime();

	const seconds = Math.round(diff / 1000);
	if (seconds < 60) {
		return relativeTimeFormatter.format(-1 * seconds, "seconds");
	}

	const minutes = Math.round(seconds / 60);
	if (minutes < 60) {
		return relativeTimeFormatter.format(-1 * minutes, "minutes");
	}

	const hours = Math.round(minutes / 60);
	if (hours < 24) {
		return relativeTimeFormatter.format(-1 * hours, "hours");
	}

	const days = Math.round(hours / 24);
	if (days < 7) {
		return relativeTimeFormatter.format(-1 * days, "days");
	}

	const weeks = Math.round(days / 7);
	if (weeks < 4) {
		return relativeTimeFormatter.format(-1 * weeks, "weeks");
	}
	
	return date.toLocaleDateString();
}