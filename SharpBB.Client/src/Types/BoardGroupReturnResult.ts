interface BoardGroupReturnResult {
	id: number,
	title: string,
	description: string,
	boards: {
		uuid: string,
		name: string,
		description: string,
		dateCreated: string,
		latestActivity: {
			uuid: string,
			title: string,
			content: null,
			dateTime: string,
			childrenUuids: unknown[],
			parentUuid: null, by: string | null
		}, topicCount:number, repliesCount:number
	}[]
}

export default BoardGroupReturnResult;