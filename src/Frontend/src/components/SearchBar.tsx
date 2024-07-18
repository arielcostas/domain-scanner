import { FormEvent, ReactElement, useState } from "react";

interface SearchBarProps {
	name: string;
	label: string;
	placeholder: string;
	onSearch: (searchInput: string) => void;
}

export function SearchBar({ name, label, placeholder, onSearch }: SearchBarProps): ReactElement<SearchBarProps, string> {
	const [searchInput, setSearchInput] = useState<string>('');

	const handleSubmit = (e: FormEvent<HTMLFormElement>) => {
		console.log('searchInput', searchInput);
		e.preventDefault();
		onSearch(searchInput);
	}

	return (
		<form onSubmit={handleSubmit}>
			<label htmlFor={`search-${name}`}>
				{label}
			</label>
			<input type="search"
				placeholder={placeholder ?? label}
				id={`search-${name}`}
				value={searchInput} onChange={e => setSearchInput(e.target.value)} />
			<button type="submit">Search</button>
		</form>

	)
}